using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.SimpleLogging.Repositories
{
    public abstract class ALogRepository : ILogRepository
    {
        public string ConnectionString { get; set; }
        public string ApplicationName { get; set; }
        public string UserName { get; set; }

        public abstract void Log(string Level, Exception ex);

        public abstract void Log(string Level, string Message);

        public abstract long LogStartTime(string Office, int Token);

        public abstract long LogStopTime(long id);
    }
}
