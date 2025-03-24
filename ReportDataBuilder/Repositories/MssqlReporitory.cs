using Microsoft.Data.SqlClient;
using ReportDataBuilder.objects;
using ILogger = ReportDataBuilder.SimpleLogging.Logger.ILogger;

namespace ReportDataBuilder.Repositories
{
    public class MssqlReporitory : Repository
    {
        public MssqlReporitory(ILogger logger)
        {
            this.Logger = logger;
        }

        public override async Task<List<string>> GetColumnNamesAsync(string connectionstring, string query, string column)
        {
            using SqlConnection connection = new(connectionstring);
            await connection.OpenAsync();
            using SqlCommand command = new(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            List<string> Returnable = [];
            while (reader.Read())
            {
                if (reader[column] != null)
                    Returnable.Add(reader[column].ToString() ?? "");
            }
            return Returnable;
        }

        public override async Task<List<ViewObject>> ReadDataAsync(string connectionstring, string query, List<string> columnNames, DateTime? lastdate)
        {
            using SqlConnection connection = new(connectionstring);
            SqlCommand command = new(query, connection);
            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();
            List<ViewObject> objects = [];
            while (reader.Read())
            {
                ViewObject vo = new();
                foreach (var col in columnNames)
                {
                    vo.paras.Add(col, new ParamValue(reader.GetFieldType(reader.GetOrdinal(col)).ToString(), reader.GetValue(reader.GetOrdinal(col)).ToString() ?? ""));
                }
                objects.Add(vo);
            }
            return objects;
        }
    }
}
