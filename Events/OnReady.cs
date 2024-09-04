using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StarportBot.Events;

public partial class EventHandler
{
    private async Task OnReady()
    {
        await RegisterCommandsAsync();
    }
    
    private async Task RegisterCommandsAsync()
    {
        logger.LogInformation("Registering commands...");

        var commandsRegistered = environment.IsDevelopment() 
            ? await RegisterCommandsLocallyAsync()
            : await RegisterCommandsGloballyAsync();

        logger.LogInformation("Registered {count} commands!", commandsRegistered);
    }
    
    private async Task<int> RegisterCommandsLocallyAsync()
    {
        await Client.Rest.DeleteAllGlobalCommandsAsync();
        var result = await interactionService.RegisterCommandsToGuildAsync(config.DiscordSettings.DevGuildId);
        return result.Count;
    }

    private async Task<int> RegisterCommandsGloballyAsync()
    {
        await Client.Rest.BulkOverwriteGuildCommands([], config.DiscordSettings.DevGuildId);
        var result = await interactionService.RegisterCommandsGloballyAsync();
        return result.Count;
    }
}