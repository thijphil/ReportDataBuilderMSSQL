using MySql.Data.MySqlClient;
using ReportDataBuilder.SimpleLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.SimpleLogging.Repositories
{
    public class LogRepository : ALogRepository
    {
        public LogRepository(string ConnectionString, string ApplicationName)
        {
            this.ConnectionString = ConnectionString;

            this.ApplicationName = ApplicationName;
            //this.UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            UserName = Environment.UserName;
        }
        public override void Log(string Level, Exception ex)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
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
                MySqlCommand command = new MySqlCommand(LogCommand, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public override void Log(string Level, string Message)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                Console.WriteLine(Message);
                string LogCommand = QueryBuilder.InsertErrorLog(
                   ApplicationName,
                   Level,
                   Message,
                   UserName,
                   "",
                   "");

                MySqlCommand command = new MySqlCommand(LogCommand, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public override long LogStartTime(string Office, int Token)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                string LogCommand = QueryBuilder.InsertTimeLog(
                   ApplicationName,
                   Office,
                   Token);

                MySqlCommand command = new MySqlCommand(LogCommand, connection);
                connection.Open();
                command.ExecuteNonQuery();
                return command.LastInsertedId;
            }
        }

        public override long LogStopTime(long id)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                string LogCommand = QueryBuilder.UpdateTimeLog(id);

                MySqlCommand command = new MySqlCommand(LogCommand, connection);
                connection.Open();
                command.ExecuteNonQuery();
                return command.LastInsertedId;
            }
        }
    }
}
