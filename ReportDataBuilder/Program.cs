using ReportDataBuilder;
using ReportDataBuilder.JsonSettings;
using ReportDataBuilder.SimpleLogging.Logger;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
