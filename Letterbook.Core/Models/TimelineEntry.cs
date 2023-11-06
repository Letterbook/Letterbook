namespace Letterbook.Core.Models;

public class TimelineEntry
{
    // Time is when the post was added to the feed, not when it was created
    // Those are often more or less the same, except in the case of boosts
    public DateTime Time { get; set; } = DateTime.UtcNow;  //index
    public ActivityObjectType Type { get; set; }
    public required string EntityId { get; set; }  //index
    public required string AudienceKey { get; set; } //index
    public string? AudienceName { get; set; }
    public required string[] CreatedBy { get; set; }
    public required string Authority { get; set; }
    public string? BoostedBy { get; set; }
    public DateTime CreatedDate { get; set; }

    // private TimelineEntry()
    // {
        // Type = default!;
        // EntityId = default!;
        // AudienceKey = default!;
        // AudienceName = default!;
        // CreatedBy = default!;
        // Authority = default!;
        // BoostedBy = default!;
        // CreatedDate = DateTime.UtcNow;
    // }
}
