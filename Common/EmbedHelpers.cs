using Discord;

namespace StarportBot.Common;

public static class EmbedHelpers
{
    public static EmbedBuilder WithSuccess(this EmbedBuilder builder) =>
        builder
            .WithAuthor("Success", Constants.Icons.Check)
            .WithColor(Constants.Colors.Success);
    
    public static EmbedBuilder WithFailure(this EmbedBuilder builder) =>
        builder
            .WithAuthor("Failure", Constants.Icons.Cross)
            .WithColor(Constants.Colors.Danger);
}