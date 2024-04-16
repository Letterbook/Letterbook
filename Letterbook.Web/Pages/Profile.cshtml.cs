using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Api.Mappers;
using Letterbook.Core;
using Models = Letterbook.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

public class Profile : PageModel
{
	private readonly IProfileService _profiles;
	private readonly Mapper _mapper;

	public required string Handle { get; set; }
	public required Models.Profile Prof { get; set; }
	public required string Json { get; set; }

	public Profile(IProfileService profiles, MappingConfigProvider mappingConfigs)
	{
		_profiles = profiles;
		_mapper = new Mapper(mappingConfigs.Profiles);
	}

	public async Task<IActionResult> OnGet(string handle)
	{
		var found = await _profiles.As(User.Claims).FindProfiles(handle);
		if (found.FirstOrDefault() is not { } profile)
			return NotFound();
		Prof = profile;
		Handle = handle;

		var options = new JsonSerializerOptions { WriteIndented = true };
		Json = JsonSerializer.Serialize(_mapper.Map<FullProfileDto>(Prof), options);

		return Page();
	}
}