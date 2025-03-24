using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.SimpleLogging
{
    public class InitController
    {

        private string ConnectionString;

        public InitController(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;

        }

        public void Init()
        {
            CheckOrCreateTableExists();
        }

        private void CheckOrCreateTableExists()
        {

            var database = ConnectionString.Split("Database=")[1].Split(";")[0];

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string comm = QueryBuilder.CheckTableExisits(database);

                MySqlCommand command = new MySqlCommand(comm, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                bool timeTableExisits = false;
                bool errorTableExisits = false;
                try
                {
                    while (reader.Read())
                    {
                        var tablename = reader["TABLE_NAME"].ToString();
                        if (tablename.ToLower().Equals("applicationlog"))
                        {
                            timeTableExisits = true;
                        }
                        if (tablename.ToLower().Equals("errorlog"))
                        {
                            errorTableExisits = true;
                        }
                    }
                    reader.Close();
                }
                finally
                {
                    reader.Close();
                }

                if (!timeTableExisits)
                    CreateTimeLogTable();

                if (!errorTableExisits)
                    CreateErrorLogTable();
            }
        }
        private void CreateErrorLogTable()
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                MySqlCommand command = new MySqlCommand(QueryBuilder.CreateErrorTable(), connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        private void CreateTimeLogTable()
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                MySqlCommand command = new MySqlCommand(QueryBuilder.CreateTimeTable(), connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
