namespace Letterbook.Adapter.Db;

public class DesignDbOptions : DbOptions
{
    // TODO: consider reading this from app settings
    public DesignDbOptions()
    {
        Host = "localhost";
        Port = "5432";
        Username = "letterbook";
        Password = "letterbookpw";
        UseSsl = false;
        // It's convenient to have an integration database, so that actor/object IDs
        // can refer to the proper URL for integration services to refer to them.
        Database = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Sandcastle" ? "letterbook_int" : "letterbook";
    }

    public override string GetConnectionString()
    {
        return base.GetConnectionString() + "Include Error Detail=true";
    }
}