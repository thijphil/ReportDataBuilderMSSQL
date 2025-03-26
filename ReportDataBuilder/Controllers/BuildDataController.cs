using ReportDataBuilder.objects;
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
            List<string> objectNames = await Repository.GetExsistingTables(ReceivingConnectionString, ReceivingDatabaseName);
            foreach (var objectName in objectNames)
            {
                var id = Logger.LogStart(objectName, 0);
                try
                {
                    var viewColumnNameQuery = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{objectName}' and TABLE_CATALOG = '{SendingDatabaseName}';";
                    var columnsNames = await Repository.GetColumnNamesAsync(SendingConnectionString, viewColumnNameQuery, "COLUMN_NAME");                    
                    await SyncCreatedRows( columnsNames, objectName);   
                    await SyncUpdatedRows(columnsNames, objectName);
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

        private async Task SyncCreatedRows(List<string> ColumnsNames, string ObjectName) 
        {
            Logger.LogInfo($"Fetching latest datetime from {ObjectName}");
            string CreatedFilterCol = HasCreatedFilter(ColumnsNames) ? "CreatedDatetimeFilter" : "CreatedDatetime";
            var CreatedLastDate = await Repository.GetLatestDateTimeAsync(ReceivingConnectionString, ObjectName, CreatedFilterCol, ActionEnum.Create);

            Logger.LogInfo($"Creating Query");
            string CreatedColNames = StringBuilder.CreateColumnListMsSql(ColumnsNames);
            var CreatedReadDataQuery = CreatedLastDate == null ? $"select {CreatedColNames} from {ObjectName}" : $"select {CreatedColNames} from {ObjectName} where {CreatedFilterCol} > '{CreatedLastDate}'";
            Logger.LogInfo("Fetching data from view");
            var objects = await Repository.ReadDataAsync(SendingConnectionString, CreatedReadDataQuery, ColumnsNames);
            Logger.LogInfo("Writing data to table");
            await Repository.WriteDataAsync(ReceivingConnectionString, ObjectName, objects, ActionEnum.Create);
        }
        private async Task SyncUpdatedRows(List<string> ColumnsNames, string ObjectName)
        {
            Logger.LogInfo($"Fetching latest datetime from {ObjectName}");
            string UpdatedFilterCol = "";
            if (HasUpdatedFilter(ColumnsNames))
                UpdatedFilterCol = "UpdatedDatetimeFilter";
            else
                UpdatedFilterCol = HasUpdateFilter(ColumnsNames) ? "UpdateDatetimeFilter" : "UpdatedDateTime";


            var UpdatedLastDate = await Repository.GetLatestDateTimeAsync(ReceivingConnectionString, ObjectName, UpdatedFilterCol, ActionEnum.Update);

            //var UpdatedLastDate = DateTime.Now.Date.AddDays(-10000);

            Logger.LogInfo($"Creating Query");
            string UpdatedColNames = StringBuilder.CreateColumnListMsSql(ColumnsNames);
            var UpdatedReadDataQuery = $"select {UpdatedColNames} from {ObjectName} where {UpdatedFilterCol} > '{UpdatedLastDate}'";

            Logger.LogInfo("Fetching data from view");
            var objects = await Repository.ReadDataAsync(SendingConnectionString, UpdatedReadDataQuery, ColumnsNames);
            Logger.LogInfo("Writing data to table");
            await Repository.WriteDataAsync(ReceivingConnectionString, ObjectName, objects, ActionEnum.Update);
        }
    }
}
