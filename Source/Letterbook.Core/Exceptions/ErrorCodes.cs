namespace Letterbook.Core.Exceptions;

/// <summary>
/// </summary>
[Flags]
public enum ErrorCodes
{
	None = 0,
	// Core Errors - 0x0001 to 0x0800
	InvalidRequest = 0x1,
	WrongAuthority = 0x2,
	DuplicateEntry = 0x4,
	MissingData = 0x8,
	PermissionDenied = 0x10,
	InternalError = 0x0800,
	// ActivityPub Errors - 0x0_1000 to 0x8_0000
	UnknownVocabulary = 0x1000,
	UnknownSemantics = 0x2000,
	// Client Errors - 0x0010_0000 to 0x8F00_0000
	PeerError = 0x010_0000, // An internal error in a peer system
	NetworkError = 0x020_0000,
}