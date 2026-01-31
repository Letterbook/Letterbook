using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Letterbook.Api.Dto;
using Letterbook.Core;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Letterbook.Cli.Adapters;

/*

	Requires API to be running:

		dotnet watch run --project Source/Letterbook.Api --launch-profile dev

*/
public class NetworkAccountService(HttpClient httpClient, ILogger<NetworkAccountService> log) : IAccountService
{
	public async Task<IdentityResult> RegisterAccount(string email, string handle, string password, string inviteCode)
	{
		var url = "lb/v1/user_account/register";

		var payload = new RegistrationRequest
		{
			Handle = handle,
			InviteCode = inviteCode,
			Password = password,
			ConfirmPassword = password,
			Email = email,
		};

		log.LogDebug($"Registering account at <{httpClient.BaseAddress}{url}> with payload <{JsonConvert.SerializeObject(payload)}>");

		var reply = await httpClient.PostAsync(url, JsonContent.Create(payload));

		log.LogDebug(reply.StatusCode.ToString());
		log.LogDebug(await reply.Content.ReadAsStringAsync());

		if (reply.StatusCode == HttpStatusCode.BadRequest)
		{
			IdentityError[]? errors = JsonConvert.DeserializeObject<IdentityError[]>(await reply.Content.ReadAsStringAsync());

			// [!] Can't create IdentityResult with more than error as the `IdentityResult.Failed` overload is internal.
			return IdentityResult.Failed(errors!.First());
		}

		return reply.IsSuccessStatusCode ? IdentityResult.Success : IdentityResult.Failed();
	}

	#region Not implemented

	public Task<AccountIdentity> AuthenticatePassword(string email, string password)
	{
		throw new NotImplementedException();
	}

	public Task<IdentityResult> ChangePassword(Guid id, string currentPassword, string newPassword)
	{
		throw new NotImplementedException();
	}

	public Task<Account?> LookupAccount(Guid id)
	{
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<Account> FindAccounts(string email)
	{
		throw new NotImplementedException();
	}

	public Task<Account?> FirstAccount(string email)
	{
		throw new NotImplementedException();
	}

	public Task<string> GenerateChangeEmailToken(Guid accountId, string email)
	{
		throw new NotImplementedException();
	}

	public Task<string> GenerateInviteCode(Guid accountId, int uses = 0, DateTimeOffset? expiration = null)
	{
		throw new NotImplementedException();
	}

	public Task DeliverPasswordResetLink(string email, string linkPartial)
	{
		throw new NotImplementedException();
	}

	public Task<IdentityResult> ChangeEmailWithToken(string oldEmail, string newEmail, string token)
	{
		throw new NotImplementedException();
	}

	public Task<IdentityResult> ChangePasswordWithToken(string email, string password, string token)
	{
		throw new NotImplementedException();
	}

	public Task<bool> AddLinkedProfile(Guid accountId, Profile profile, IEnumerable<ProfileClaim> claims)
	{
		throw new NotImplementedException();
	}

	public Task<bool> UpdateLinkedProfile(Guid accountId, Profile profile, IEnumerable<ProfileClaim> claims)
	{
		throw new NotImplementedException();
	}

	public Task<bool> RemoveLinkedProfile(Guid accountId, Profile profile)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<Claim>> LookupClaims(Account account)
	{
		throw new NotImplementedException();
	}

	#endregion
}