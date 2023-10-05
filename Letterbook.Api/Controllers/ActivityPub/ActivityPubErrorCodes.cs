namespace Letterbook.Api.Controllers.ActivityPub;

/// <summary>
/// Range 0x01000 to 0xF0000
/// </summary>
[Flags]
public enum ActivityPubErrorCodes
{
    None = 0x0000,
    UnknownVocabulary = 0x1000,
    UnknownSemantics = 0x2000
}