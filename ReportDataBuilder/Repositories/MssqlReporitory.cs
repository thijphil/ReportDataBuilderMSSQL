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
    public class MssqlReporitory : Reporitory
    {


        public override List<string> GetDatabases(string connectionstring)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                string query = "SELECT name FROM sys.databases;";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<string> Returnable = new List<string>();
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["name"]}");
                            if(reader["name"] != null)
                                Returnable.Add(reader["name"].ToString());
                        }
                        return Returnable;
                    }
                }
            }
        }

        public override List<string> GetNames(string connectionstring, string query, string column)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<string> Returnable = new List<string>();
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader[column]}");
                            if (reader[column] != null)
                                Returnable.Add(reader[column].ToString());
                        }
                        return Returnable;
                    }
                }
            }
        }

        public override List<ViewObject> ReadData(string connectionstring, string query, List<string> columnNames, DateTime? lastdate)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<ViewObject> objects = new List<ViewObject>();
                    while (reader.Read())
                    {
                        // Access your columns here, for example:
                        ViewObject vo = new ViewObject();
                        foreach (var col in columnNames)
                        {
                            vo.paras.Add(col, new ParamValue(reader.GetFieldType(reader.GetOrdinal(col)).ToString(), reader.GetValue(reader.GetOrdinal(col)).ToString()));
                        }
                        objects.Add(vo);
                        // Add more columns as needed
                    }
                    return objects;
                }
            }
        }
    }
}
