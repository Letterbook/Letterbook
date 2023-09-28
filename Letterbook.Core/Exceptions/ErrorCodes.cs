namespace Letterbook.Core.Exceptions;

[Flags]
public enum ErrorCodes
{
    None = 0,
    InvalidRequest = 0x1,
    WrongAuthority = 0x2,
    DuplicateEntry = 0x4,
    MissingData = 0x8,
    
}