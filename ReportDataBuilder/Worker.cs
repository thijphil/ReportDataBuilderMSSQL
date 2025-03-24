using ReportDataBuilder.Controllers;

namespace ReportDataBuilder
{
    public class Worker(BuildDataController mssqlController) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    if (_logger.IsEnabled(LogLevel.Information))
            //    {
            //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    }
            //    await Task.Delay(1000, stoppingToken);
            //}
            await mssqlController.BuildDataAsync();
            //new MysqlController().BuildData();
        }
    }
}
