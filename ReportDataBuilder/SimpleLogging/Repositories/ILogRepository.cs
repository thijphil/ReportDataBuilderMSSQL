using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.SimpleLogging.Repositories
{
    public interface ILogRepository
    {
        void Log(string Level, Exception ex);
        void Log(string Level, string Message);
        long LogStartTime(string Office, int Token);
        long LogStopTime(long id);
    }
}
