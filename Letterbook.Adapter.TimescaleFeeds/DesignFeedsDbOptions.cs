namespace Letterbook.Adapter.TimescaleFeeds;

public class DesignFeedsDbOptions : FeedsDbOptions
{
    public DesignFeedsDbOptions()
    {
        Host = "localhost";
        Port = "5432";
        Username = "letterbook";
        Password = "letterbookpw";
        UseSsl = false;
        Database = "letterbook_feeds";
    }
}
