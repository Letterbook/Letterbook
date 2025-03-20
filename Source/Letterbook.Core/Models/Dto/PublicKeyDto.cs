namespace Letterbook.Core.Models.Dto;

public class PublicKeyDto
{
	public string? Label { get; set; }
	public required string Family { get; set; }
	public required string PublicKeyPem { get; set; }
	public required DateTimeOffset Created { get; set; }
	public required DateTimeOffset Expires { get; set; }
	public required Uri FediId { get; set; }
}