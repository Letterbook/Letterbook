using System.Data;
using Letterbook.Core.Exceptions;

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
        if (!string.IsNullOrEmpty(ConnectionString)) return ConnectionString;
        if (string.IsNullOrEmpty(Host)) throw new ConfigException("Host cannot be null, specify a Host or ConnectionString");
        if (string.IsNullOrEmpty(Port)) throw new ConfigException("Port cannot be null, specify a Port or ConnectionString");
        if (string.IsNullOrEmpty(Username))
            throw new ConfigException("Username cannot be null, specify a Username or ConnectionString");
        if (string.IsNullOrEmpty(Password))
            throw new ConfigException("Password cannot be null, specify a Password or ConnectionString");
        if (string.IsNullOrEmpty(Database))
            throw new ConfigException("Database cannot be null, specify a Database or ConnectionString");
        return
            $"Server={Host};Port={Port};Database={Database};User Id={Username};Password={Password};SSL Mode={(UseSsl is true ? "Require" : "Prefer")};";
    }
}