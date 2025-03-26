using ReportDataBuilder.Repositories;
using ILogger = ReportDataBuilder.SimpleLogging.Logger.ILogger;

namespace ReportDataBuilder.Controllers
{
    public abstract class Controller : IController
    {
        public readonly IConfigurationSection DatabaseSettings;
        public readonly ILogger Logger;
        public readonly IRepository Repository;
        public readonly string ReceivingDatabaseName;
        public readonly string ReceivingConnectionString;
        public readonly string SendingDatabaseName;
        public readonly string SendingConnectionString;
        public Controller(IConfiguration configuration, ILogger logger, IRepository repository)
        {
            DatabaseSettings = configuration.GetSection("DatabaseSettings");
            Logger = logger;
            Repository = repository;
            ReceivingDatabaseName = DatabaseSettings["ReceivingDatabaseName"] ?? "";
            ReceivingConnectionString = $"{DatabaseSettings["ReceivingConnectionString"]}Database={ReceivingDatabaseName};";
            SendingDatabaseName = DatabaseSettings["SelectedDatabase"] ?? "";
            SendingConnectionString = $"{DatabaseSettings["SendingConnectionString"]}Database={SendingDatabaseName};";
        }
        public abstract Task BuildDataAsync();
        public static bool HasCreatedFilter(List<string> columnNames) => columnNames.Contains("CreatedDatetimeFilter");
        public static bool HasUpdatedFilter(List<string> columnNames) => columnNames.Contains("UpdatedDatetimeFilter");
        public static bool HasUpdateFilter(List<string> columnNames) => columnNames.Contains("UpdateDatetimeFilter");
    }
}
