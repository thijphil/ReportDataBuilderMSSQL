using ReportDataBuilder;
using ReportDataBuilder.Controllers;
using ReportDataBuilder.Repositories;
using ReportDataBuilder.SimpleLogging.Logger;
using ILogger = ReportDataBuilder.SimpleLogging.Logger.ILogger;

var builder = Host.CreateApplicationBuilder(args);
var configurationSection = builder.Configuration.GetSection("DatabaseSettings");
var connString = $"{configurationSection["ReceivingConnectionString"] ?? ""}Database={configurationSection["ReceivingDatabaseName"] ?? ""};";
builder.Services.AddSingleton<ILogger>(_ => new Logger(connString, configurationSection["AppId"] ?? ""));
builder.Services.AddSingleton<IRepository, MssqlReporitory>();
builder.Services.AddSingleton<BuildDataController>();
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
