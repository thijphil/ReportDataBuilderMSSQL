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
        List<string> GetDatabases(string connectionstring);
        List<ViewObject> ReadData(string connectionstring, string query, List<string> columnNames, DateTime? lastdate);
        List<string> GetNames(string connectionstring, string query, string column);
        bool CheckTableExists(string connectionstring, string tableName, string dbName);
        void WriteData(string connectionstring, string tablename, List<ViewObject> objects);
        void TruncateData(string connectionstring, string tablename);
        DateTime? GetLatestDateTime(string connectionstring, string tableName, string filterCol);
    }
}
