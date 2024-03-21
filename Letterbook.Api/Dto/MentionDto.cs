using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Api.Dto;

public class MentionDto
{
    public string? Mentioned { get; set; }
    public MentionVisibility Visibility { get; set; }
}