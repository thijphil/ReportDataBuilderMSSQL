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
        Task<List<ViewObject>> ReadDataAsync(string connectionstring, string query, List<string> columnNames);
        Task<List<string>> GetColumnNamesAsync(string connectionstring, string query, string column);
        Task WriteDataAsync(string connectionstring, string tablename, List<ViewObject> objects, ActionEnum actionEnum);
        Task TruncateDataAsync(string connectionstring, string tablename);
        Task<DateTime?> GetLatestDateTimeAsync(string connectionstring, string tableName, string filterCol, ActionEnum actionEnum);
        Task<List<string>> GetExsistingTables(string ConnectionString, string receivingDatabaseName);
    }
}
