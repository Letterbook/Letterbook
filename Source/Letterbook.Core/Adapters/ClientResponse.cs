using System.Net;

namespace Letterbook.Core.Adapters;

public record ClientResponse<T>
{
	public required T? Data { get; set; }
	public required Uri? DeliveredAddress { get; set; }
	public required HttpStatusCode StatusCode { get; set; }
}