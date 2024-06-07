using Letterbook.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;

namespace Letterbook.Web.Pages;

public class ProfileEdit : PageModel
{
    private readonly IProfileService _profiles;
	private readonly CoreOptions _options;
    
   	public string? Handle { get; set; }

	public ProfileEdit(IProfileService profiles, IOptions<CoreOptions> options)
	{
		_profiles = profiles;
		_options = options.Value;
		Description = "";
		DisplayName = "";
	}
    
	[BindProperty]
	[Required]
	[StringLength(60)]
	public string DisplayName { get; set; }
	
	[BindProperty]
	[StringLength(1000)]
	public string Description { get; set; }
	
    public async Task<IActionResult> OnGet(string handle)
    {
        var found = await _profiles.As(User.Claims).FindProfiles(handle);
		if (found.FirstOrDefault() is not { } profile)
			return NotFound();

		Handle = handle;
		DisplayName = profile.DisplayName;
		Description = profile.Description;
        return Page();
    }
	
	public async Task<IActionResult> OnPostAsync(string handle)
	{
		var found = await _profiles.As(User.Claims).FindProfiles(handle);
		if (found.FirstOrDefault() is not { } profile)
			return NotFound();
		
		if (ModelState.IsValid) {
			await _profiles.As(User.Claims).UpdateDisplayName(profile.Id, DisplayName);
			await _profiles.As(User.Claims).UpdateDescription(profile.Id, Description);
			return RedirectToPage("Profile", new { handle = handle });
		}
		return Page();
	}
}