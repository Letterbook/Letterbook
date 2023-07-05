using System.Data;

namespace Letterbook.Adapter.Db;

public class DbOptions
{
    public const string ConfigKey = "Letterbook.Database";

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
        if (Host == null) throw new NoNullAllowedException("Host cannot be null, specify a Host or ConnectionString");
        if (Port == null) throw new NoNullAllowedException("Port cannot be null, specify a Port or ConnectionString");
        if (Username == null)
            throw new NoNullAllowedException("Username cannot be null, specify a Username or ConnectionString");
        if (Password == null)
            throw new NoNullAllowedException("Password cannot be null, specify a Password or ConnectionString");
        return
            $"Server={Host};Port={Port};Database={Database};User Id={Username};Password={Password};SSL Mode={(UseSsl ?? true ? "Require" : "Prefer")};";
    }
}