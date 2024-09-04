using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace StarportBot.Events;

public partial class EventHandler
{
    private async Task OnJoinGuildAsync(SocketGuildUser arg)
    {
        if (arg.Guild.Id != config.DiscordSettings.DevGuildId)
        {
            return;
        }

        if (arg.DisplayName == "!")
        {
            logger.LogInformation("Person joined with a display name of '!' - assumed spammer, banning...");
            await arg.Guild.BanUserAsync(arg.Id);
        }
    }
}