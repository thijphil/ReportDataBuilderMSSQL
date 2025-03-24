using MySql.Data.MySqlClient;
using static Mysqlx.Notice.Warning.Types;

namespace ReportDataBuilder.SimpleLogging.Repositories
{
    public class LogRepository : ALogRepository
    {
        public LogRepository(string ConnectionString, string ApplicationName)
        {
            this.ConnectionString = ConnectionString;
            this.ApplicationName = ApplicationName;
            this.UserName = Environment.UserName;
        }
        public override void Log(string Level, Exception ex)
        {
            Console.WriteLine($"{Level}: {ex}");
            using MySqlConnection connection = new(ConnectionString);
            string innerEx = string.Empty;
            if (ex.InnerException != null)
                innerEx = ex.InnerException.Message;
            innerEx = ex.GetType().Name;

            string stackTrace = string.Empty;
            if (ex.StackTrace != null)
                stackTrace = ex.StackTrace;

            string LogCommand = QueryBuilder.InsertErrorLog(
                ApplicationName,
                Level,
                ex.Message,
                UserName,
                stackTrace,
                innerEx);
            MySqlCommand command = new(LogCommand, connection);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public override void Log(string Level, string Message)
        {
            Console.WriteLine($"{Level}: {Message}");
            using MySqlConnection connection = new(ConnectionString);            
            string LogCommand = QueryBuilder.InsertErrorLog(
               ApplicationName,
               Level,
               Message,
               UserName,
               "",
               "");
            MySqlCommand command = new(LogCommand, connection);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public override long LogStartTime(string Office, int Token)
        {
            
            using MySqlConnection connection = new(ConnectionString);
            string LogCommand = QueryBuilder.InsertTimeLog(
               ApplicationName,
               Office,
               Token);
            MySqlCommand command = new(LogCommand, connection);
            connection.Open();
            command.ExecuteNonQuery();
            Console.WriteLine($"Start ID: {command.LastInsertedId}; Office: {Office}; Token: {Token}; {DateTime.Now}");
            return command.LastInsertedId;
        }

        public override long LogStopTime(long id)
        {            
            using MySqlConnection connection = new(ConnectionString);
            string LogCommand = QueryBuilder.UpdateTimeLog(id);
            MySqlCommand command = new(LogCommand, connection);
            connection.Open();
            command.ExecuteNonQuery();
            Console.WriteLine($"Stop ID: {id}; {DateTime.Now}");
            return command.LastInsertedId;
        }
    }
}
