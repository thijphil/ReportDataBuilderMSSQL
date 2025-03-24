using MySql.Data.MySqlClient;
using ReportDataBuilder.objects;
using System.Data;
using StringBuilder = ReportDataBuilder.StringOperations.StringBuilder;

namespace ReportDataBuilder.Repositories
{
    public abstract class Repository : IRepository
    {

        public abstract Task<List<string>> GetDatabasesAsync(string connectionstring);
        public abstract Task<List<string>> GetNamesAsync(string connectionstring, string query, string column);
        public abstract Task<List<ViewObject>> ReadDataAsync(string connectionstring, string query, List<string> columnNames, DateTime? lastdate);
        
        public async Task<bool> CheckTableExistsAsync(string connectionstring, string tableName, string dbName)
        {
            string query = "SELECT count(*) as amount " +
                "FROM information_schema.TABLES " +
                $"WHERE (TABLE_SCHEMA = '{dbName}') " +
                $"AND (TABLE_NAME = '{tableName}')";
            using MySqlConnection connection = new(connectionstring);
            try
            {
                connection.Open();
                MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    int amount = reader.GetInt32("amount");
                    return amount > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (ex.StackTrace != null)
                    Console.WriteLine(ex.StackTrace);
            }
            return false;
        }
        public async Task WriteDataAsync(string connectionstring, string tablename, List<ViewObject> objects)
        {
            using MySqlConnection connection = new(connectionstring);
            await connection.OpenAsync();
            foreach (var item in objects)
            {
                try
                {
                    string columnNames = StringBuilder.CreateColumnList(item);
                    string columnParams = StringBuilder.CreateColumnParamsList(item);
                    string values = StringBuilder.CreateValueList(item);
                    columnNames = columnNames.Replace(" ", "");
                    columnParams = columnParams.Replace(" ", "");
                    string query = $"INSERT INTO {tablename}" +
                        "(" +
                        columnNames +
                        ")" +
                        "VALUES" +
                        $"(" +
                        columnParams +
                        ");";

                    MySqlCommand command = new(query, connection);
                    var parameters = new Dictionary<string, ParamValue>();
                    foreach (var param in item.paras)
                    {
                        parameters.Add(param.Key.Replace(" ", ""), param.Value);
                    }
                    foreach (var para in parameters)
                    {
                        string val = string.Empty;
                        if (string.IsNullOrEmpty(para.Value.Value))
                            val = "0Error";
                        else
                            val = para.Value.Value;
                        switch (para.Value.Type)
                        {
                            case "System.Int64":
                                if (!val.Equals("0Error"))
                                    command.Parameters.AddWithValue($"@{para.Key}", int.Parse(val));
                                else
                                    command.Parameters.AddWithValue($"@{para.Key}", 0);
                                break;
                            case "System.Boolean":
                                if (!val.Equals("0Error"))
                                {
                                    if (bool.Parse(val))
                                        command.Parameters.AddWithValue($"@{para.Key}", 1);
                                    else
                                        command.Parameters.AddWithValue($"@{para.Key}", 0);
                                }
                                else
                                    command.Parameters.AddWithValue($"@{para.Key}", 0);
                                break;
                            case "System.Int32":
                                if (!val.Equals("0Error"))
                                    command.Parameters.AddWithValue($"@{para.Key}", int.Parse(val));
                                else
                                    command.Parameters.AddWithValue($"@{para.Key}", 0);
                                break;
                            case "System.DateTime":
                                if (!val.Equals("0Error"))
                                    command.Parameters.AddWithValue($"@{para.Key}", DateTime.Parse(val));
                                else
                                    command.Parameters.AddWithValue($"@{para.Key}", null);//DateTime.MinValue
                                break;
                            case "System.Decimal":
                                if (!val.Equals("0Error"))
                                    command.Parameters.AddWithValue($"@{para.Key}", decimal.Parse(val));
                                else
                                    command.Parameters.AddWithValue($"@{para.Key}", 0);
                                break;
                            case "System.Guid":
                            case "System.String":
                                if (!val.Equals("0Error"))
                                    command.Parameters.AddWithValue($"@{para.Key}", para.Value.Value.ToString());
                                else
                                    command.Parameters.AddWithValue($"@{para.Key}", "");
                                break;
                            case "System.Double":
                                if (!val.Equals("0Error"))
                                    command.Parameters.AddWithValue($"@{para.Key}", double.Parse(val));
                                else
                                    command.Parameters.AddWithValue($"@{para.Key}", 0);
                                break;
                            default:
                                break;
                        }
                    }
                    // Execute the query
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    // Check if any rows were affected
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Data inserted successfully!");
                    }
                    else
                    {
                        Console.WriteLine("No data inserted!");
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Message.Contains("Duplicate entry"))
                    {
                        Console.WriteLine($"Error: {ex.Message} Table:{tablename}");
                    }
                    else
                    {
                        Console.WriteLine($"Error: {ex.Message} Table:{tablename}");
                        if (ex.StackTrace != null)
                            Console.WriteLine(ex.StackTrace);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message} Table:{tablename}");
                    if (ex.StackTrace != null)
                        Console.WriteLine(ex.StackTrace);
                }
            }
        }
        public async Task TruncateDataAsync(string connectionstring, string tablename)
        {
            using MySqlConnection connection = new(connectionstring);
            try
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"TRUNCATE TABLE {tablename}";
                await command.ExecuteNonQueryAsync();
                Console.WriteLine("Table truncated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (ex.StackTrace != null)
                    Console.WriteLine(ex.StackTrace);
            }
        }

        public virtual async Task<DateTime?> GetLatestDateTimeAsync(string connectionstring, string tableName, string filterCol)
        {
            using MySqlConnection connection = new(connectionstring);
            try
            {
                await connection.OpenAsync();

                using var command = new MySqlCommand($"SELECT MAX({filterCol}) as CreatedDatetimeFilter FROM {tableName}", connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(reader.GetValue("CreatedDatetimeFilter").ToString()))
                            return null;
                        DateTime lastdate = reader.GetDateTime("CreatedDatetimeFilter");
                        return lastdate;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        if (string.IsNullOrEmpty(reader.GetValue("CreatedDatetimeFilter").ToString()))
                            return null;
                        string dateString = reader.GetString("CreatedDatetimeFilter");
                        DateTime date = DateTime.ParseExact(dateString, "dd-MM-yyyy", null);
                        return date;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }


       
    }
}
