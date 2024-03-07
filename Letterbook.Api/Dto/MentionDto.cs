using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Api.Dto;

public class MentionDto
{
    public Uuid7 Mentioned { get; set; }
    public MentionVisibility Visibility { get; set; }
}