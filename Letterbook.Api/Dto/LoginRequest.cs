using System.ComponentModel.DataAnnotations;

namespace Letterbook.Api.Dto;

public class LoginRequest
{
    [EmailAddress] public required string Email { get; set; }
    public required string Password { get; set; }
    // anti-forgery token?
}