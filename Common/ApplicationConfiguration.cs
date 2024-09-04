namespace StarportBot.Common;

[Serializable]
public class ApplicationConfiguration
{
    public bool Debug { get; set; }
    public DiscordSettings DiscordSettings { get; set; } = new();
    public ImageSettings ImageSettings { get; set; } = new();
    public DatabaseSettings DatabaseSettings { get; set; } = new();
}

[Serializable]
public class DiscordSettings
{
    public string Token { get; set; } = string.Empty;
    public ulong DevGuildId { get; set; }
    public ulong VoidChannelId { get; set; }
}

[Serializable]
public class ImageSettings
{
    public string WebDavUrl { get; set; } = string.Empty;
    public string WebDavRoot { get; set; } = string.Empty;
    public string WebDavUsername { get; set; } = string.Empty;
    public string WebDavPassword { get; set; } = string.Empty;
    public string FoxShare { get; set; } = "Foxes";
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string Database { get; set; } = "the-starport";
    public string LostUserCollection { get; set; } = "lost-users";
}