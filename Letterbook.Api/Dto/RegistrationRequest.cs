using System.ComponentModel.DataAnnotations;

namespace Letterbook.Api.Dto;

public class RegistrationRequest
{
    public required string Handle { get; set; }
    [EmailAddress] public required string Email { get; set; }
    public required string Password { get; set; }
    [Compare(nameof(Password))] public required string ConfirmPassword { get; set; }
}