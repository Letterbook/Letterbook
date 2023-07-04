using System.ComponentModel.DataAnnotations;

namespace Letterbook.Adapter.Db.Entities;

public class Audience
{
    [Required]
    public Uri? Id { get; set; }
}