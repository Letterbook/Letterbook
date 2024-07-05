// ReSharper disable BuiltInTypeReferenceStyle
namespace Letterbook.Core.Models;

public enum ProfileClaim
{
	None,

	Owner,		// The creator/owner of the profile. Shorthand for unrestricted access
	Guest,		// Has some claims delegated by the owner. All claims must be verified from the source
}