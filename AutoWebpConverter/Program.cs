using Megacorp.AutoWebpConverter;
using Megacorp.AutoWebpConverter.Services;
using Megacorp.AutoWebpConverter.Configuration;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .AddJsonFile("appsettings.json")
        .Build();

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder =>
    {
        builder.Sources.Clear();
        builder.AddConfiguration(configuration);
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddEventLog();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddHostedService<AutoWebpConverterBackgroundService>()
            .Configure<AutoWebpConverterConfiguration>(configuration.GetSection("AutoWebpConverter"));
    })
    .UseWindowsService();

IHost host = builder.Build();
host.Run();