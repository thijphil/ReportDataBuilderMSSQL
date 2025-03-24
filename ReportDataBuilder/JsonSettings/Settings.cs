using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.JsonSettings
{
    public class Settings
    {
        public string AppId { get; set; }
        public string ReceivingDatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string ReceivingConnectionString { get; set; }
        public string LoggingConnectionString { 
            get {
                return $"{ConnectionString}Database={ReceivingDatabaseName};";
            } 
        }
        public List<string> SelectedDatabases { get; set; }
    }
}
