using MySql.Data.MySqlClient;
using ReportDataBuilder.objects;
using System.Data;
using StringBuilder = ReportDataBuilder.StringOperations.StringBuilder;
using ILogger = ReportDataBuilder.SimpleLogging.Logger.ILogger;

namespace ReportDataBuilder.Repositories
{
    public abstract class Repository : IRepository
    {
        public required ILogger Logger { get; set; }
        public abstract Task<List<string>> GetColumnNamesAsync(string connectionstring, string query, string column);
        public abstract Task<List<ViewObject>> ReadDataAsync(string connectionstring, string query, List<string> columnNames);
        public async Task<List<string>> GetExsistingTables(string ConnectionString) 
        {
            List<string> tableNames = [];
            using MySqlConnection connection = new(ConnectionString);
            await connection.OpenAsync();
            string query = "SHOW TABLES";
            MySqlCommand command = new(query, connection);

            using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                tableNames.Add(reader.GetString(0));
            }
            
            return tableNames;
        }
        public async Task WriteDataAsync(string connectionstring, string tablename, List<ViewObject> objects, ActionEnum actionEnum)
        {
            using MySqlConnection connection = new(connectionstring);
            await connection.OpenAsync();
            foreach (var item in objects)
            {
                try
                {
                    item.paras.Add("FetchDate", new ParamValue("System.DateTime", DateTime.Now.ToString()));
                    item.paras.Add("Action", new ParamValue("System.String", actionEnum.ToString()));
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
                    //command.Parameters.AddWithValue("FetchDate", DateTime.Now);
                    //command.Parameters.AddWithValue("Action", actionEnum.ToString());
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        Logger.LogConsole($"Data inserted successfully into {tablename}!");
                    }
                    else
                    {
                        Logger.LogConsole($"No data inserted into {tablename}!");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error: {ex.Message} Table:{tablename}");
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
                Logger.LogInfo("Table truncated successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError("Error: " + ex.Message);
                if (ex.StackTrace != null)
                    Logger.LogInfo(ex.StackTrace);
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
                    catch
                    {
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
                Logger.LogError("Error: " + ex.Message);
            }
            return null;
        }


       
    }
}
