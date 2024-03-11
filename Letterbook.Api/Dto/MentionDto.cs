using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Api.Dto;

public class MentionDto
{
	/// <example>0672016s27hx3fjxmn5ic1hzq</example>
    public string? Mentioned { get; set; }
    public MentionVisibility Visibility { get; set; }
}