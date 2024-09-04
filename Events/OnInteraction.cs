using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using StarportBot.Common;

namespace StarportBot.Events;

public partial class EventHandler
{
    private async Task OnInteractionCreated(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(Client, interaction);
            await interactionService.ExecuteCommandAsync(context, provider);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Exception occurred whilst attempting to handle interaction.");
        }
    }

    private static async Task OnInteractionExecuted(ICommandInfo command, IInteractionContext context, IResult result)
    {
        if (string.IsNullOrEmpty(result.ErrorReason))
        {
            return;
        }

        var builder = new EmbedBuilder()
            .WithDescription(result.ErrorReason);
        if (result.IsSuccess)
        {
            builder.WithSuccess();
        }
        else
        {
            builder.WithFailure();
        }

        if (context.Interaction.HasResponded)
        {
            await context.Interaction.FollowupAsync(embed: builder.Build()).ConfigureAwait(false);
        }
        else
        {
            await context.Interaction.RespondAsync(embed: builder.Build()).ConfigureAwait(false);
        }
    }
}