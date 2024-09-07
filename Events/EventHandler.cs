using System.Reflection;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StarportBot.Common;

namespace StarportBot.Events;

public partial class EventHandler(DiscordSocketClient client,
    ILogger<EventHandler> logger,
    IServiceProvider provider,
    InteractionService interactionService,
    IHostEnvironment environment,
    ApplicationConfiguration config) : DiscordClientService(client, logger)
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Client.Ready += OnReady;
        Client.UserJoined += OnJoinGuildAsync;
        Client.MessageReceived += OnMessageReceived;
        Client.InteractionCreated += OnInteractionCreated;

        await using var scope = provider.CreateAsyncScope();
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), scope.ServiceProvider);

        interactionService.InteractionExecuted += OnInteractionExecuted;
    }
}