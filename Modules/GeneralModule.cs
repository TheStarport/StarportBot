using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using StarportBot.Common;

namespace StarportBot.Modules;

public class GeneralModule : InteractionModuleBase
{
    [SlashCommand("about", "Shows information about the bot")]
    public async Task AboutAsync()
    {
        var app = await Context.Client.GetApplicationInfoAsync();

        var guilds = await Context.Client.GetGuildsAsync();

        var embed = new EmbedBuilder()
            .WithTitle(app.Name)
            .WithDescription(app.Description)
            .AddField("Servers", guilds?.Count.ToString() ?? "N/A", true)
            .AddField("Version", Assembly.GetExecutingAssembly().GetName().Version, true)
            .WithAuthor(app.Owner.Username, app.Owner.GetDisplayAvatarUrl())
            .WithFooter(string.Join(" · ", app.Tags.Select(t => '#' + t)))
            .WithColor(Constants.Colors.Primary)
            .Build();
        
        await RespondAsync(embed: embed);
    }
}