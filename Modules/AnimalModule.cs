#if false // Disabled for now
using System.Net;
using Discord;
using Discord.Interactions;
using StarportBot.Common;
using WebDAVClient;

namespace StarportBot.Modules;
public class AnimalModule(ApplicationConfiguration configuration) : InteractionModuleBase
{
    [SlashCommand("fox", "Grab a random fox image!")]
    public async Task FoxAsync()
    {
        try
        {
            var (info, file) = await RandomImage(configuration.ImageSettings.FoxShare);
            await RespondWithFileAsync(new FileAttachment(file, info), "Look, a fox!");
        }
        catch (Exception ex)
        {
            if (configuration.Debug)
            {
                await RespondAsync(ex.Message);
                return;
            }

            var embed = new EmbedBuilder()
                .WithFailure()
                .WithDescription("Something went wrong while contacting the api")
                .Build();
            
            await RespondAsync(embed: embed);
        }
    }

    private async Task<(string, Stream?)> RandomImage(string share)
    {
        using IClient webDav = new Client(new NetworkCredential
        {
            UserName = configuration.ImageSettings.WebDavUsername, 
            Password = configuration.ImageSettings.WebDavPassword
        });

        webDav.Server = configuration.ImageSettings.WebDavUrl;
        webDav.BasePath = configuration.ImageSettings.WebDavRoot;

        var files = await webDav.List(share);
        var fileEntry = files.Where(x => !x.IsHidden && !x.IsCollection).RandomElement();

        return (fileEntry.DisplayName, await webDav.Download(fileEntry.Href));
    }
}
#endif