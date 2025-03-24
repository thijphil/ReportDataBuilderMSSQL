using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.CRUD;
using ReportDataBuilder.JsonSettings;
using ReportDataBuilder.objects;
using ReportDataBuilder.Repositories;
using ReportDataBuilder.SimpleLogging.Logger;
using ReportDataBuilder.StringOperations;
using ILogger = ReportDataBuilder.SimpleLogging.Logger.ILogger;

namespace ReportDataBuilder.Controllers
{
    public class MysqlController : Controller
    {
        private IRepository repository;
        private ILogger logger;

        public MysqlController()
        {
            logger = new Logger(GlobalSettings.settings.LoggingConnectionString, GlobalSettings.settings.AppId);
            repository = new MysqlRepository();
        }
        public override void BuildData()
        {
            string ReceivingDatabaseName = GlobalSettings.settings.ReceivingDatabaseName;
            string connectionString = GlobalSettings.settings.ConnectionString;
            List<string> selectedDatabases = GlobalSettings.settings.SelectedDatabases;
            string receivingConnectionString = $"{connectionString}Database={ReceivingDatabaseName};";
            logger.LogInfo("Fetching Databases");
            var existingDatabases = repository.GetDatabases(connectionString);
            logger.LogInfo($"Found {existingDatabases.Count} Databases");
            List<string> databases = [];
            logger.LogInfo("Checking if selected databases exist");
            foreach (var item in selectedDatabases)
            {
                if (existingDatabases.Contains(item))
                    databases.Add(item);
            }

            foreach (var database in databases)
            {

                string tempConnectionString = $"{connectionString}Database={database};";
                string query = $"SHOW FULL TABLES IN {database} WHERE TABLE_TYPE LIKE 'VIEW';";
                var views = repository.GetNames(tempConnectionString, query, $"Tables_in_{database}");

                foreach (var view in views)
                {
                    string tablename = StringBuilder.CreateTableName(view);
                    if (!repository.CheckTableExists(receivingConnectionString, tablename, ReceivingDatabaseName))
                        continue;
                    var id = logger.LogStart(tablename, 0);
                    try
                    {

                        var viewColumnNameQuery = $"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = '{view}' and TABLE_SCHEMA = '{database}';";
                        var columnsNames = repository.GetNames(tempConnectionString, viewColumnNameQuery, "COLUMN_NAME");

                        //logger.LogInfo("Truncating data");
                        //repository.TruncateData(receivingConnectionString, tablename);

                        logger.LogInfo($"Fetching latest datetime from {tablename}");
                        var lastDate = repository.GetLatestDateTime(receivingConnectionString, tablename, "ViewDate");
                        logger.LogInfo($"Creating Query");
                        var readDataQuery = $"select {StringBuilder.CreateColumnListMySql(columnsNames)} from {view} where ViewDate > @datetime";
                        logger.LogInfo("Fetching data from view");
                        var objects = repository.ReadData(tempConnectionString, readDataQuery, columnsNames, lastDate);
                        logger.LogInfo("Writing data to table");
                        repository.WriteData(receivingConnectionString, tablename, objects);
                        logger.LogStop(id);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Oops, i had an error in {tablename}");
                        logger.LogError(ex);
                        logger.LogStop(id);
                    }
                }
            }
            Environment.Exit(0);
        }
    }
}
