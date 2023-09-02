namespace Letterbook.Adapter.Db.NavigationModels;

public class FollowerRelation
{
    public Guid Id { get; set; }
    public Models.Profile Subject { get; set; }
    public Models.Profile Follows { get; set; }
    public DateTime Date { get; set; }

    private FollowerRelation()
    {
        Id = Guid.Empty;
        Subject = default!;
        Follows = default!;
        Date = DateTime.UtcNow;
    }
}