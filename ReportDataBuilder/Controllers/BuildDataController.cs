using ReportDataBuilder.Repositories;
using ReportDataBuilder.StringOperations;
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
                    string FilterCol = HasFilter(columnsNames) ? "CreatedDatetimeFilter" : "CreatedDatetime";
                    Logger.LogInfo($"Fetching latest datetime from {objectName}");
                    var lastDate = await Repository.GetLatestDateTimeAsync(ReceivingConnectionString, objectName, FilterCol);
                    Logger.LogInfo($"Creating Query");
                    string colNames = StringBuilder.CreateColumnListMsSql(columnsNames);
                    var readDataQuery = lastDate == null? $"select {colNames} from {objectName}" : $"select {colNames} from {objectName} where {FilterCol} > '{lastDate}'";
                    Logger.LogInfo("Fetching data from view");
                    var objects = await Repository.ReadDataAsync(SendingConnectionString, readDataQuery, columnsNames, lastDate);
                    Logger.LogInfo("Writing data to table");
                    await Repository.WriteDataAsync(ReceivingConnectionString, objectName, objects);
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
    }
}
