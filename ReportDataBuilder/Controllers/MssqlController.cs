using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.CRUD;
using ReportDataBuilder.JsonSettings;
using ReportDataBuilder.objects;
using ReportDataBuilder.Repositories;
using ReportDataBuilder.SimpleLogging.Logger;
using ReportDataBuilder.StringOperations;
using System.Xml;
using ILogger = ReportDataBuilder.SimpleLogging.Logger.ILogger;

namespace ReportDataBuilder.Controllers
{
    public class MssqlController : Controller
    {
        private IRepository repository;
        //private ILogger logger;
        public MssqlController()
        {
            //logger = new Logger(GlobalSettings.settings.LoggingConnectionString, GlobalSettings.settings.AppId);
            repository = new MssqlReporitory();
        }
        public override void BuildData()
        {
            Console.SetOut(new CustomTextWriter(Console.Out));
            string ReceivingDatabaseName = GlobalSettings.settings.ReceivingDatabaseName;
            string connectionString = GlobalSettings.settings.ConnectionString;
            List<string> selectedDatabases = GlobalSettings.settings.SelectedDatabases;
            string receivingConnectionString = $"{GlobalSettings.settings.ReceivingConnectionString}Database={ReceivingDatabaseName};";
            //logger.LogInfo("Fetching Databases");
            var existingDatabases = repository.GetDatabases(connectionString);
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
                List<string> AllItems = new List<string>();
                string tempConnectionString = $"{connectionString}Database={database};";
                //string viewsQuery = $"SELECT name FROM sys.views;";
                //var views = repository.GetNames(tempConnectionString, viewsQuery, $"name");
                string tablesQuery = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;";
                var tables = repository.GetNames(tempConnectionString, tablesQuery, $"TABLE_NAME");

                //AllItems.AddRange(views);
                AllItems.AddRange(tables);

                var items = new List<string>();
                Console.WriteLine($"DBBY : GetMore");
                foreach (var view in AllItems)
                {
                    string tablename = StringBuilder.CreateTableName(view);
                    if (repository.CheckTableExists(receivingConnectionString, tablename, ReceivingDatabaseName))
                    {
                        Console.WriteLine($"{tablename} : {view}");
                        items.Add(view);
                    }
                }

                foreach (var view in items)
                {
                    string tablename = StringBuilder.CreateTableName(view);
                    if (!repository.CheckTableExists(receivingConnectionString, tablename, ReceivingDatabaseName))
                        continue;
                    //var id = logger.LogStart(tablename, 0);
                    try
                    {

                        var viewColumnNameQuery = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{view}' and TABLE_CATALOG = '{database}';";
                        var columnsNames = repository.GetNames(tempConnectionString, viewColumnNameQuery, "COLUMN_NAME");
                        string FilterCol = HasFilter(columnsNames) ? "CreatedDatetimeFilter" : "CreatedDatetime";
                        //logger.LogInfo("Truncating data");
                        //repository.TruncateData(receivingConnectionString, tablename);

                        //logger.LogInfo($"Fetching latest datetime from {tablename}");
                        var lastDate = repository.GetLatestDateTime(receivingConnectionString, tablename, FilterCol);
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
                        var objects = repository.ReadData(tempConnectionString, readDataQuery, columnsNames, lastDate);
                        //logger.LogInfo("Writing data to table");
                        repository.WriteData(receivingConnectionString, tablename, objects);
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
