using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Letterbook.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Models.Account> _signInManager;
        private readonly UserManager<Models.Account> _userManager;
        private readonly IAccountService _accounts;
        private readonly ILogger<LoginModel> _logger;

        public required IList<AuthenticationScheme> ExternalLogins { get; set; } = null!;
        public required string ReturnUrl { get; set; } = null!;

        [BindProperty]
        public required InputModel Input { get; set; } = null!;

        [TempData]
        public required string ErrorMessage { get; set; } = null!;

        [SetsRequiredMembers]
        public LoginModel(ILogger<LoginModel> logger, SignInManager<Models.Account> signInManager, UserManager<Models.Account> userManager, IAccountService accounts)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accounts = accounts;
            _logger = logger;
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public required string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public required string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This is less than ideal, from a security perspective.
                // It leaks whether an email address is in use.
                // Consider login with username?
                var user = await _accounts.FirstAccount(Input.Email);
                if (user is null)
                {
	                ModelState.AddModelError(string.Empty, "Invalid username or password");
	                return Page();
                }

                var succeeded = await _userManager.CheckPasswordAsync(user, Input.Password);
                if(succeeded)
                {
	                if (user.TwoFactorEnabled)
	                {
		                // TODO: set activeProfile on 2fa login
		                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
	                }

	                var profileClaims = user.LinkedProfiles
		                .Select(l => new Claim($"profile:{l.Profile.GetId25()}", string.Join(",", l.Claims)));
	                var defaultProfile = user.LinkedProfiles.OrderBy(pc => pc.GetId())
		                .FirstOrDefault(pc => pc.Claims.Contains(Models.ProfileClaim.Owner));
	                if (defaultProfile is not null)
		                profileClaims = profileClaims.Append(new Claim("activeProfile", defaultProfile.Profile.GetId25()));

	                await _signInManager.SignInWithClaimsAsync(user, Input.RememberMe, profileClaims);
	                _logger.LogDebug("Account {User} signed in and granted {ProfileClaims}", user.UserName, profileClaims);
	                return LocalRedirect(returnUrl);
                }

                await _userManager.AccessFailedAsync(user);
                var lockedOut = await _userManager.IsLockedOutAsync(user);
                if (lockedOut)
                {
                    _logger.LogWarning("User {Name} account locked out", user.UserName);
                    return RedirectToPage("./Lockout");
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return Page();
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
