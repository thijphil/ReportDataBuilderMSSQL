using ReportDataBuilder.Repositories;
using ReportDataBuilder.StringOperations;

namespace ReportDataBuilder.Controllers
{
    public class MssqlController(IConfiguration configuration) : Controller
    {
        private readonly MssqlReporitory repository = new();
        private readonly IConfigurationSection _databaseSettings = configuration.GetSection("DatabaseSettings");

        public override async Task BuildDataAsync()
        {
            string ReceivingDatabaseName = _databaseSettings["ReceivingDatabaseName"] ?? "";
            string connectionString = _databaseSettings["ConnectionString"] ?? "";
            List<string> selectedDatabases = _databaseSettings.GetSection("SelectedDatabases").Get<List<string>>() ?? [];
            string receivingConnectionString = $"{_databaseSettings["ReceivingConnectionString"]}Database={ReceivingDatabaseName};";
            //logger.LogInfo("Fetching Databases");
            var existingDatabases = await repository.GetDatabasesAsync(connectionString);
            //logger.LogInfo($"Found {existingDatabases.Count} Databases");
            List<string> databases = [];
            //logger.LogInfo("Checking if selected databases exist");
            foreach (var item in selectedDatabases)
            {
                if (existingDatabases.Contains(item))
                    databases.Add(item);
            }

            foreach (var database in databases)
            {
                List<string> AllItems = [];
                string tempConnectionString = $"{connectionString}Database={database};";
                //string viewsQuery = $"SELECT name FROM sys.views;";
                //var views = repository.GetNames(tempConnectionString, viewsQuery, $"name");
                string tablesQuery = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;";
                var tables = await repository.GetNamesAsync(tempConnectionString, tablesQuery, $"TABLE_NAME");

                //AllItems.AddRange(views);
                AllItems.AddRange(tables);

                var items = new List<string>();
                Console.WriteLine($"DBBY : GetMore");
                foreach (var view in AllItems)
                {
                    string tablename = StringBuilder.CreateTableName(view);
                    if (await repository.CheckTableExistsAsync(receivingConnectionString, tablename, ReceivingDatabaseName))
                    {
                        Console.WriteLine($"{tablename} : {view}");
                        items.Add(view);
                    }
                }

                foreach (var view in items)
                {
                    string tablename = StringBuilder.CreateTableName(view);
                    if (!await repository.CheckTableExistsAsync(receivingConnectionString, tablename, ReceivingDatabaseName))
                        continue;
                    //var id = logger.LogStart(tablename, 0);
                    try
                    {

                        var viewColumnNameQuery = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{view}' and TABLE_CATALOG = '{database}';";
                        var columnsNames = await repository.GetNamesAsync(tempConnectionString, viewColumnNameQuery, "COLUMN_NAME");
                        string FilterCol = HasFilter(columnsNames) ? "CreatedDatetimeFilter" : "CreatedDatetime";
                        //logger.LogInfo("Truncating data");
                        //repository.TruncateData(receivingConnectionString, tablename);

                        //logger.LogInfo($"Fetching latest datetime from {tablename}");
                        var lastDate = await repository.GetLatestDateTimeAsync(receivingConnectionString, tablename, FilterCol);
                        //logger.LogInfo($"Creating Query");
                        //var readDataQuery = $"select {StringBuilder.CreateColumnListMsSql(columnsNames)} from {view} where ViewDate > @datetime";
                        var readDataQuery = "";
                        if (lastDate == null)
                        {
                            readDataQuery = $"select {StringBuilder.CreateColumnListMsSql(columnsNames)} from {view}";
                        }
                        else 
                        {
                            readDataQuery = $"select {StringBuilder.CreateColumnListMsSql(columnsNames)} from {view} where {FilterCol} > '{lastDate}'";
                        }
                       // logger.LogInfo("Fetching data from view");
                        var objects = await repository.ReadDataAsync(tempConnectionString, readDataQuery, columnsNames, lastDate);
                        //logger.LogInfo("Writing data to table");
                        await repository.WriteDataAsync(receivingConnectionString, tablename, objects);
                        //logger.LogStop(id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Oops, i had an error in {tablename}");
                        Console.WriteLine(ex);
                        if (ex.StackTrace != null)
                            Console.WriteLine(ex.StackTrace);
                    }
                }
            }
            Environment.Exit(0);
        }        
    }
}
