namespace Letterbook.Adapter.Db;

public class DesignDbOptions : DbOptions
{
    public DesignDbOptions()
    {
        Host = "localhost";
        Port = "5432";
        Username = "letterbook";
        Password = "letterbookpw";
        UseSsl = false;
        Database = "letterbook";
    }
}