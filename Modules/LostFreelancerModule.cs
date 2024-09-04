using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Humanizer;
using Humanizer.Localisation;
using StarportBot.Common;
using StarportBot.Models;
using StarportBot.Services;

namespace StarportBot.Modules;

[RequireContext(ContextType.Guild)]
public class LostFreelancerModule(ApplicationConfiguration configuration, LostFreelancerCollection lostFreelancerCollection) : InteractionModuleBase
{
    [DefaultMemberPermissions(GuildPermission.BanMembers)]
    [SlashCommand("addlostfreelancer", "Adds a new lost user to the list")]
    public async Task AddLostFreelancerAsync(IUser user, IGuildUser? banningAdmin = null)
    {
        if (Context.Guild.Id != configuration.DiscordSettings.DevGuildId)
        {
            await RespondAsync("This command cannot be used here.");
            return;
        }
        
        var admin = banningAdmin ?? (Context.User as IGuildUser)!;

        var lostUser = new LostUser()
        {
            BannedUser = user.Id,
            BanningAdmin = admin.Id
        };
        
        var bannedMsg = await BanIfPossibleAsync(user) 
            ? ""
            : "\nFailed to ban target user. Manual ban may be required.";

        var lastLostUser = await lostFreelancerCollection.GetLastLostFreelancer();
        if (!await lostFreelancerCollection.AddNewLostFreelancerAsync(lostUser))
        {
            await RespondAsync("Failed to add new freelancer. Username or AdminName was missing." + bannedMsg);
            return;
        }
        
        if (lastLostUser is null)
        {
            await RespondAsync("New lost freelancer added!" + bannedMsg);
            return;
        }
        
        await RespondAsync($"New lost freelancer added. The last lost freelancer was seen: " +
                           $"{(lostUser.Time - lastLostUser.Time).Humanize(maxUnit: TimeUnit.Month, minUnit: TimeUnit.Day, precision: 7)} ago."
                           + bannedMsg);
    }

    [SlashCommand("getlostfreelancer", "Gets the last lost Freelancer")]
    public async Task GetLastLostFreelancerAsync()
    {
        var lastFreelancer = await lostFreelancerCollection.GetLastLostFreelancer();
        if (lastFreelancer is null)
        {
            EmbedBuilder err = new();
            err
                .WithTitle("Not Found")
                .WithDescription("There is no lost freelancer currently found. None have been added?")
                .WithFailure();
                
            await RespondAsync(embed: err.Build());
            return;
        }

        string message = $"The last user was banned on {lastFreelancer.Time:yyyy/MM/dd}\n";
        
        IUser? adminUser = await Context.Client.GetUserAsync(lastFreelancer.BanningAdmin);
        IUser? lastUser = await Context.Client.GetUserAsync(lastFreelancer.BannedUser);
        
        message += lastUser is null
            ? "Their account has since been deleted.\n"
            : $"Their name is currently set to: {lastUser.GlobalName ?? lastUser.Username}\n";
        
        message += adminUser is null
            ? "They were banned by an admin who's account has since been deleted.\n"
            : $"They were banned by: {adminUser.GlobalName ?? adminUser.Username}";
            
        EmbedBuilder builder = new();
        builder
            .WithTitle("Last Lost Freelancer")
            .WithDescription(message)
            .WithSuccess();

        await RespondAsync(embed: builder.Build());
    }

    [SlashCommand("listlostfreelancer", "Gets the last lost Freelancer")]
    public async Task GetLostFreelancerListAsync()
    {
        
    }

    private async Task<bool> BanIfPossibleAsync(IUser user)
    {
        try
        {
            if (user is SocketGuildUser socketGuildUser)
            {
                await socketGuildUser.BanAsync();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}