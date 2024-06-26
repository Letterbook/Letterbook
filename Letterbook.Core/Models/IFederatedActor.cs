namespace Letterbook.Core.Models;

public interface IFederatedActor : IFederated
{
	IList<SigningKey> Keys { get; set; }
}