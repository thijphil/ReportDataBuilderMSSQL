using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportDataBuilder.SimpleLogging.Logger
{
    public interface ILogger
    {
        void LogDebug(Exception ex);
        void LogDebug(string Message);
        void LogDebug(string[] Messages);
        void LogError(Exception ex);
        void LogError(string Message);
        void LogError(string[] Messages);
        void LogFatal(Exception ex);
        void LogFatal(string Message);
        void LogFatal(string[] Messages);
        void LogInfo(Exception ex);
        void LogInfo(string Message);
        void LogInfo(string[] Messages);
        void LogConsole(Exception ex);
        void LogConsole(string Message);
        void LogConsole(string[] Messages);
        void LogTrace(Exception ex);
        void LogTrace(string Message);
        void LogTrace(string[] Messages);
        void LogWarn(Exception ex);
        void LogWarn(string Message);
        long LogStart(string Office, int Token);
        void LogStop(long id);

    }
}
