using ReportDataBuilder.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.Repositories
{
    public interface IRepository
    {
        Task<List<string>> GetDatabasesAsync(string connectionstring);
        Task<List<ViewObject>> ReadDataAsync(string connectionstring, string query, List<string> columnNames, DateTime? lastdate);
        Task<List<string>> GetNamesAsync(string connectionstring, string query, string column);
        Task<bool> CheckTableExistsAsync(string connectionstring, string tableName, string dbName);
        Task WriteDataAsync(string connectionstring, string tablename, List<ViewObject> objects);
        Task TruncateDataAsync(string connectionstring, string tablename);
        Task<DateTime?> GetLatestDateTimeAsync(string connectionstring, string tableName, string filterCol);
    }
}
