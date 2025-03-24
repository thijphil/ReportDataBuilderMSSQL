using ReportDataBuilder.SimpleLogging.Repositories;
using System.Runtime.ExceptionServices;

namespace ReportDataBuilder.SimpleLogging.Logger
{
    public class Logger : ILogger
    {
        private ILogRepository logRepository;
        public Logger(string ConnectionString, string ApplicationName)
        {
            logRepository = new LogRepository(ConnectionString, ApplicationName);
            InitController initController = new InitController(ConnectionString);
            initController.Init();
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
            AppDomain.CurrentDomain.FirstChanceException += FirstChanceExceptionEventHandler;
        }
        private void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
                LogFatal((Exception)e.ExceptionObject);
        }
        private void FirstChanceExceptionEventHandler(object sender, FirstChanceExceptionEventArgs e)
        {
            LogError(e.Exception);
        }

        public void LogDebug(Exception ex)
        {
            logRepository.Log("Debug", ex);
        }
        public void LogError(Exception ex)
        {
            logRepository.Log("Error", ex);
        }
        public void LogFatal(Exception ex)
        {
            logRepository.Log("Fatal", ex);
        }
        public void LogInfo(Exception ex)
        {
            logRepository.Log("Info", ex);
        }
        public void LogTrace(Exception ex)
        {
            logRepository.Log("Trace", ex);
        }
        public void LogWarn(Exception ex)
        {
            logRepository.Log("Warn", ex);
        }
        public void LogDebug(string Message)
        {
            logRepository.Log("Debug", Message);
        }
        public void LogError(string Message)
        {
            logRepository.Log("Error", Message);
        }
        public void LogFatal(string Message)
        {
            logRepository.Log("Fatal", Message);
        }
        public void LogInfo(string Message)
        {
            logRepository.Log("Info", Message);
        }
        public void LogTrace(string Message)
        {
            logRepository.Log("Trace", Message);
        }
        public void LogWarn(string Message)
        {
            logRepository.Log("Warn", Message);
        }
        public void LogDebug(string[] Messages)
        {
            logRepository.Log("Debug", MakeLogMessage(Messages));
        }
        public void LogError(string[] Messages)
        {
            logRepository.Log("Debug", MakeLogMessage(Messages));
        }
        public void LogFatal(string[] Messages)
        {
            logRepository.Log("Debug", MakeLogMessage(Messages));
        }
        public void LogInfo(string[] Messages)
        {
            logRepository.Log("Debug", MakeLogMessage(Messages));
        }
        public void LogTrace(string[] Messages)
        {
            logRepository.Log("Debug", MakeLogMessage(Messages));
        }
        private string MakeLogMessage(string[] messages)
        {
            string message = string.Empty;
            foreach (var messageItem in messages)
            {
                message += messageItem + "/r/n";
            }
            return message;
        }

        public long LogStart(string Office, int Token)
        {
            return logRepository.LogStartTime(Office, Token);
        }

        public void LogStop(long id)
        {
            logRepository.LogStopTime(id);
        }
    }
}