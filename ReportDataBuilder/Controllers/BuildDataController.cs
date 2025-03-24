using ReportDataBuilder.Repositories;
using ReportDataBuilder.StringOperations;
using System.Security.AccessControl;
using ILogger = ReportDataBuilder.SimpleLogging.Logger.ILogger;

namespace ReportDataBuilder.Controllers
{
    public class BuildDataController(IConfiguration configuration, ILogger Logger, IRepository Repository) : Controller(configuration, Logger, Repository)
    {
        public override async Task BuildDataAsync()
        {
            foreach (var objectName in await Repository.GetExsistingTables(ReceivingConnectionString))
            {
                var id = Logger.LogStart(objectName, 0);
                try
                {
                    var viewColumnNameQuery = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{objectName}' and TABLE_CATALOG = '{SendingDatabaseName}';";
                    var columnsNames = await Repository.GetColumnNamesAsync(SendingConnectionString, viewColumnNameQuery, "COLUMN_NAME");
                    
                    Logger.LogInfo($"Fetching latest datetime from {objectName}");
                    string CreatedFilterCol = HasCreatedFilter(columnsNames) ? "CreatedDatetimeFilter" : "CreatedDatetime";
                    var CreatedLastDate = await Repository.GetLatestDateTimeAsync(ReceivingConnectionString, objectName, CreatedFilterCol);

                    Logger.LogInfo($"Creating Query");
                    string CreatedColNames = StringBuilder.CreateColumnListMsSql(columnsNames);
                    var CreatedReadDataQuery = CreatedLastDate == null? $"select {CreatedColNames} from {objectName}" : $"select {CreatedColNames} from {objectName} where {CreatedFilterCol} > '{CreatedLastDate}'";
                    await SyncCreatedRows(CreatedReadDataQuery, columnsNames, objectName);

                    Logger.LogInfo($"Fetching latest datetime from {objectName}");
                    string UpdatedFilterCol = HasCreatedFilter(columnsNames) ? "UpdateDatetimeFilter" : "UpdateDateTime";
                    var UpdatedLastDate = DateTime.Now;

                    Logger.LogInfo($"Creating Query");
                    string UpdatedColNames = StringBuilder.CreateColumnListMsSql(columnsNames);
                    var UpdatedReadDataQuery = $"select {UpdatedColNames} from {objectName} where {UpdatedFilterCol} > '{UpdatedLastDate}'";

                    await SyncUpdatedRows(UpdatedReadDataQuery, columnsNames, objectName);
                    Logger.LogStop(id);
                }
                catch (Exception ex)
                {
                    Logger.LogInfo($"Oops, i had an error in {objectName}");
                    Logger.LogError(ex);
                }
            }           
            Environment.Exit(0);
        }

        private async Task SyncCreatedRows(string ReadDataQuery, List<string> ColumnsNames, string ObjectName) 
        {
            Logger.LogInfo("Fetching data from view");
            var objects = await Repository.ReadDataAsync(SendingConnectionString, ReadDataQuery, ColumnsNames);
            Logger.LogInfo("Writing data to table");
            await Repository.WriteDataAsync(ReceivingConnectionString, ObjectName, objects);
        }
        private async Task SyncUpdatedRows(string ReadDataQuery, List<string> ColumnsNames, string ObjectName)
        {
            Logger.LogInfo("Fetching data from view");
            var objects = await Repository.ReadDataAsync(SendingConnectionString, ReadDataQuery, ColumnsNames);
            Logger.LogInfo("Writing data to table");
            await Repository.WriteDataAsync(ReceivingConnectionString, ObjectName, objects);
        }
    }
}
