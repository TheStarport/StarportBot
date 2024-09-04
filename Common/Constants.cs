using Discord;

namespace StarportBot.Common;

public static class Constants
{
    public static class Icons
    {
        /// <summary>
        /// The icon used to indicate a success state.
        /// </summary>
        public const string Check = "https://cdn.discordapp.com/emojis/1199976868057718876.webp?size=96&quality=lossless";

        /// <summary>
        /// The icon used to indicate an error state.
        /// </summary>
        public const string Cross = "https://cdn.discordapp.com/emojis/1199976870410715196.webp?size=96&quality=lossless";
    }
    
    /// <summary>
    /// Represents a collection of brand logos.
    /// </summary>
    public static class Logos
    {
        /// <summary>
        /// The official GitHub invertocat logo (https://github.com/logos).
        /// </summary>
        public static readonly Emote Github = "<:github:1131243046395183176>";

        /// <summary>
        /// The official Discord mark logo (https://discord.com/branding).
        /// </summary>
        public static readonly Emote Discord = "<:discord:1131243248409641000>";
    }
    
    /// <summary>
    /// Represents a constant set of predefined <see cref="Color"/> values.
    /// </summary>
    public static class Colors
    {
        /// <summary>
        /// The color used to indicate an informative state.
        /// </summary>
        public static readonly Color Primary = new(59, 163, 232);

        /// <summary>
        /// The color used to depict an emotion of positivity.
        /// </summary>
        public static readonly Color Success = new(43, 182, 115);

        /// <summary>
        /// The color used to depict an emotion of negativity.
        /// </summary>
        public static readonly Color Danger = new(231, 76, 60);
    }
}