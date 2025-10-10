using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Letterbook.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Account.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Models.Account> _signInManager;
        private readonly IAccountService _accounts;
        private readonly ILogger<LoginModel> _logger;

        public required IList<AuthenticationScheme> ExternalLogins { get; set; } = null!;
        public required string ReturnUrl { get; set; } = null!;

        [BindProperty]
        public required InputModel Input { get; set; } = null!;

        [TempData]
        public required string ErrorMessage { get; set; } = null!;

        [SetsRequiredMembers]
        public LoginModel(ILogger<LoginModel> logger, SignInManager<Models.Account> signInManager, IAccountService accounts)
        {
            _signInManager = signInManager;
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

            [Display(Name = "Remember me")]
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

                var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, true);
                if(result.Succeeded)
                {
	                if (user.TwoFactorEnabled)
	                {
		                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
	                }

	                _logger.LogDebug("Account {Name} has effective claims {Claims}", User.Identity?.Name, User.Claims);
	                return LocalRedirect(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Account {Name} locked out for repeated failed logins", user.UserName);
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
