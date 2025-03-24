using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using ReportDataBuilder.objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.Repositories
{
    public class MssqlReporitory : Repository
    {
        public override async Task<List<string>> GetDatabasesAsync(string connectionstring)
        {
            using SqlConnection connection = new(connectionstring);
            await connection.OpenAsync();
            string query = "SELECT name FROM sys.databases;";
            using SqlCommand command = new(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            List<string> Returnable = [];
            while (reader.Read())
            {
                Console.WriteLine($"{reader["name"]}");
                if (reader["name"] != null)
                    Returnable.Add(reader["name"].ToString() ?? "");
            }
            return Returnable;
        }

        public override async Task<List<string>> GetNamesAsync(string connectionstring, string query, string column)
        {
            using SqlConnection connection = new(connectionstring);
            await connection.OpenAsync();
            using SqlCommand command = new(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            List<string> Returnable = [];
            while (reader.Read())
            {
                Console.WriteLine($"{reader[column]}");
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
                // Access your columns here, for example:
                ViewObject vo = new();
                foreach (var col in columnNames)
                {
                    vo.paras.Add(col, new ParamValue(reader.GetFieldType(reader.GetOrdinal(col)).ToString(), reader.GetValue(reader.GetOrdinal(col)).ToString() ?? ""));
                }
                objects.Add(vo);
                // Add more columns as needed
            }
            return objects;
        }
    }
}
