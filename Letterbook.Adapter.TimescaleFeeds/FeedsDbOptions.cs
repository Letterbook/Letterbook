// using Letterbook.Adapter.Db.Relational;

using Letterbook.Core.Exceptions;

namespace Letterbook.Adapter.TimescaleFeeds;

public class FeedsDbOptions
{
    public new const string ConfigKey = "Letterbook.Timescale.Feeds";
    
    public string? Host { get; set; }
    public string? Port { get; set; }
    public string? Username { get; set; }
    public string? Database { get; set; }

    public bool? UseSsl { get; set; }

    // TODO: handle secrets securely
    public string? Password { get; set; }
    public string? ConnectionString { get; set; }

    public string GetConnectionString()
    {
        if (ConnectionString != null) return ConnectionString;
        if (Host == null) throw new ConfigException("Host cannot be null, specify a Host or ConnectionString");
        if (Port == null) throw new ConfigException("Port cannot be null, specify a Port or ConnectionString");
        if (Username == null)
            throw new ConfigException("Username cannot be null, specify a Username or ConnectionString");
        if (Password == null)
            throw new ConfigException("Password cannot be null, specify a Password or ConnectionString");
        if (Database == null)
            throw new ConfigException("Database cannot be null, specify a Database or ConnectionString");
        return
            $"Server={Host};Port={Port};Database={Database};User Id={Username};Password={Password};SSL Mode={(UseSsl ?? true ? "Require" : "Prefer")};";
    }
}
