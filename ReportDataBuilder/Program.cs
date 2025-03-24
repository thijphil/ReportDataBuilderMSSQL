using ReportDataBuilder;
using ReportDataBuilder.Controllers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<MssqlController>();
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
