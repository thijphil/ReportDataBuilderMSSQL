using MySql.Data.MySqlClient;
using ReportDataBuilder.objects;
using ReportDataBuilder.StringOperations;
using System.Data;

namespace ReportDataBuilder.Repositories
{
    public class MysqlRepository : Repository
    {
        public override async Task<List<string>> GetDatabasesAsync(string connectionstring)
        {
            using MySqlConnection connection = new(connectionstring);
            try
            {
                await connection.OpenAsync();
                MySqlCommand command = new("SHOW DATABASES;", connection);
                var databases = new List<string>();
                using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    string name = reader.GetString("Database");
                    if (name.Equals("information_schema"))
                        continue;
                    if (name.Equals("performance_schema"))
                        continue;
                    if (name.Equals("sys"))
                        continue;
                    if (name.Equals("mysql"))
                        continue;
                    if (name.Equals("mydb"))
                        continue;

                    databases.Add(name);
                }
                return databases;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (ex.StackTrace != null)
                    Console.WriteLine(ex.StackTrace);
            }
            return [];
        }
        public override async Task<List<ViewObject>> ReadDataAsync(string connectionstring, string query, List<string> columnNames, DateTime? lastdate)
        {
            using MySqlConnection connection = new(connectionstring);
            try
            {
                await connection.OpenAsync();
                MySqlCommand command = new(query, connection)
                {
                    CommandTimeout = 3600
                };
                if (lastdate != null)
                    command.Parameters.Add(new MySqlParameter("@datetime", lastdate));
                else
                    command.Parameters.Add(new MySqlParameter("@datetime", DateTime.MinValue));
                using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
                List<ViewObject> objects = [];
                while (reader.Read())
                {
                    ViewObject vo = new();
                    foreach (var column in columnNames)
                    {
                        vo.paras.Add(column, new ParamValue(reader.GetFieldType(reader.GetOrdinal(column)).ToString(), reader.GetValue(reader.GetOrdinal(column)).ToString() ?? ""));
                    }
                    objects.Add(vo);
                }
                await reader.CloseAsync();
                return objects;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (ex.StackTrace != null)
                    Console.WriteLine(ex.StackTrace);
            }
            return [];
        }
        public override async Task<List<string>> GetNamesAsync(string connectionstring, string query, string column)
        {

            using MySqlConnection connection = new(connectionstring);
            try
            {
                await connection.OpenAsync();
                MySqlCommand command = new(query, connection);
                List<string> views = [];
                using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    string name = reader.GetString(column);
                    views.Add(name);
                }
                return views;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (ex.StackTrace != null)
                    Console.WriteLine(ex.StackTrace);
            }
            return [];
        }           
    }
}

