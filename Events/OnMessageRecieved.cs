using System.Reflection.Metadata;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace StarportBot.Events;

public partial class EventHandler
{
    private async Task OnMessageReceived(SocketMessage msg)
    {
        if (msg.Channel.Id != config.DiscordSettings.VoidChannelId)
        {
            return;
        }

        try
        {
            var author = (msg.Author as SocketGuildUser);
            if (author!.JoinedAt.HasValue && author.JoinedAt.Value < DateTimeOffset.UtcNow.AddDays(-7))
            {
                await author.SetTimeOutAsync(TimeSpan.FromDays(1));
                await msg.DeleteAsync();
            }
            else
            {
                await author.BanAsync(pruneDays: 1, reason: "Posted in the void channel.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to ban/mute user who posted in the void channel. Missing permissions?");
        }
    }
}