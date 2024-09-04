using System.Text.Json;
using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using StarportBot.Common;
using StarportBot.Services;
using EventHandler = StarportBot.Events.EventHandler;

var builder = Host.CreateApplicationBuilder(args);

const string conf = "app_config.json";

#pragma warning disable CA1869
var serializerSettings = new JsonSerializerOptions
{
    AllowTrailingCommas = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

if (!File.Exists(conf))
{
    Console.WriteLine("Application config is missing. Writing default config.");
    Console.WriteLine("Please fill out the config before running this application.");
    ApplicationConfiguration tempConf = new();
    await File.WriteAllTextAsync(conf, JsonSerializer.Serialize(tempConf, serializerSettings));
    Environment.Exit(1);
}

var configuration = new ConfigurationBuilder()
    .AddJsonFile(conf)
    .Build();

var appConfig = configuration.Get<ApplicationConfiguration>();

if (appConfig is null)
{
    throw new InvalidOperationException("Configuration file was missing or invalid.");
}

var services = builder.Services
    .AddLogging(logBuilder =>
    {
        var logLevel = appConfig.Debug ? LogEventLevel.Debug : LogEventLevel.Information;
        var logger = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .WriteTo.Console()
            .CreateLogger();

        logBuilder.AddSerilog(logger);
    })
    .AddSingleton(appConfig);
    
services.AddDiscordHost((config, _) =>
{
    config.SocketConfig = new DiscordSocketConfig
    {
        AlwaysDownloadUsers = true,
        AlwaysDownloadDefaultStickers = true,
        AlwaysResolveStickers = true,
        LogLevel = appConfig.Debug ? LogSeverity.Debug : LogSeverity.Info,
        UseSystemClock = true,
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.MessageContent
    };
        
    config.Token = appConfig.DiscordSettings.Token;
});

services.AddInteractionService((config, _) =>
{
    config.LogLevel = LogSeverity.Debug;
    config.DefaultRunMode = RunMode.Async;
    config.UseCompiledLambda = true;
});

services.AddSingleton(new InteractiveConfig()
{
    LogLevel = LogSeverity.Warning,
    DefaultTimeout = TimeSpan.FromMinutes(5),
    ProcessSinglePagePaginators = true
});

services.AddSingleton<EventHandler>();
services.AddSingleton<InteractiveService>();

services.AddSingleton<LostFreelancerCollection>();

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<DiscordSocketClient>>();
var client = host.Services.GetRequiredService<DiscordSocketClient>();

#pragma warning disable CA2254

client.Log += async (msg) =>
{
    switch (msg.Severity)
    {
        case LogSeverity.Critical:
            logger.Log(LogLevel.Critical, msg.Exception, msg.Message);
            break;
        case LogSeverity.Error:
            logger.Log(LogLevel.Error, msg.Exception, msg.Message);
            break;
        case LogSeverity.Warning:
            logger.Log(LogLevel.Warning, msg.Message);
            break;
        case LogSeverity.Info:
            logger.Log(LogLevel.Information, msg.Message);
            break;
        case LogSeverity.Verbose:
            logger.Log(LogLevel.Trace, msg.Message);
            break;
        case LogSeverity.Debug:
            logger.Log(LogLevel.Debug, msg.Message);
            break;
        default:
            throw new InvalidOperationException(nameof(msg.Severity));
    }

    await Task.CompletedTask;
};

await host.RunAsync();
