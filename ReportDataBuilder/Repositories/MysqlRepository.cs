using MySql.Data.MySqlClient;
using ReportDataBuilder.objects;
using ReportDataBuilder.StringOperations;
using System.Data;

namespace ReportDataBuilder.Repositories
{
    public class MysqlRepository : Reporitory
    {
        public override List<string> GetDatabases(string connectionstring)
        {

            using (MySqlConnection connection = new MySqlConnection(connectionstring))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand("SHOW DATABASES;", connection);

                    var databases = new List<string>();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    if (ex.StackTrace != null)
                        Console.WriteLine(ex.StackTrace);
                }
            }
            return new List<string>();
        }
        public override List<ViewObject> ReadData(string connectionstring, string query, List<string> columnNames, DateTime? lastdate)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionstring))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.CommandTimeout = 3600;
                    if(lastdate != null)
                        command.Parameters.Add(new MySqlParameter("@datetime", lastdate));
                    else
                        command.Parameters.Add(new MySqlParameter("@datetime", DateTime.MinValue));
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        List<ViewObject> objects = new List<ViewObject>();
                        while (reader.Read())
                        {
                            ViewObject vo = new ViewObject();
                            foreach (var column in columnNames)
                            {
                                vo.paras.Add(column, new ParamValue(reader.GetFieldType(reader.GetOrdinal(column)).ToString(), reader.GetValue(reader.GetOrdinal(column)).ToString()));
                            }
                            objects.Add(vo);
                        }
                        reader.Close();
                        return objects;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    if (ex.StackTrace != null)
                        Console.WriteLine(ex.StackTrace);
                }
            }
            return new List<ViewObject>();
        }
        public override List<string> GetNames(string connectionstring, string query, string column)
        {

            using (MySqlConnection connection = new MySqlConnection(connectionstring))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(query, connection);

                    var views = new List<string>();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(column);
                            views.Add(name);
                        }
                        return views;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    if (ex.StackTrace != null)
                        Console.WriteLine(ex.StackTrace);
                }
            }
            return new List<string>();
        }           
    }
}

