using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Letterbook.Api.Dto;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Letterbook.Api.Controllers;

[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("/lb/v1/[controller]/[action]")]
public class UserAccountController : ControllerBase
{
	private readonly ILogger<UserAccountController> _logger;
	private readonly CoreOptions _coreOptions;
	private readonly string _hostSecret;
	private readonly IAccountService _accountService;

	public UserAccountController(ILogger<UserAccountController> logger, IConfiguration config, IOptions<CoreOptions> coreOptions, IAccountService accountService)
	{
		_logger = logger;
		_coreOptions = coreOptions.Value;
		_hostSecret = config.GetValue<string>("HostSecret")!;
		_accountService = accountService;
	}

	private static string MintToken(SecurityTokenDescriptor descriptor)
	{
		var handler = new JwtSecurityTokenHandler();

		return handler.WriteToken(handler.CreateToken(descriptor));
	}

	[HttpPost]
	[ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
	public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
	{
		try
		{
			var claims = await _accountService.AuthenticatePassword(loginRequest.Email, loginRequest.Password);
			if (!claims.Any()) return Unauthorized();

			// TODO: asymmetric signing key
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_hostSecret));
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Issuer = _coreOptions.BaseUri().ToString(),
				Audience = _coreOptions.BaseUri().ToString(),
				NotBefore = DateTime.UtcNow,
				Expires = DateTime.UtcNow.AddDays(28),
				SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			};
			var token = MintToken(tokenDescriptor);

			return Ok(new TokenResponse
			{
				AccessToken = token,
				ExpiresIn = (int)(tokenDescriptor.Expires - DateTime.UtcNow).Value.TotalSeconds,
				TokenType = "Bearer"
			});
		}
		catch (RateLimitException e)
		{
			return StatusCode(429, new { e.Expiration, e.Message });
		}
	}

	[HttpPost]
	[Authorize]
	public IActionResult Logout()
	{
		var controller = nameof(Logout);
		_logger.LogInformation("{Controller}", controller);

		return SignOut();
	}

	[HttpPost]
	[ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
	public async Task<IActionResult> Register([FromBody] RegistrationRequest registration)
	{
		try
		{
			var registerAccount = await _accountService
				.RegisterAccount(registration.Email, registration.Handle, registration.Password);

			if (registerAccount is null) return Forbid();
			if (!registerAccount.Succeeded) return BadRequest(registerAccount.Errors);

			return await Login(new LoginRequest { Email = registration.Email, Password = registration.Password });
		}
		catch (Exception e)
		{
			return BadRequest(e);
		}
	}
}