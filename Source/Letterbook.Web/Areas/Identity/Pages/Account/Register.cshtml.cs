using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Encodings.Web;
using Letterbook.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
#pragma warning disable CS8604 // Possible null reference argument.

namespace Letterbook.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Models.Account> _signInManager;
        private readonly IAccountService _accounts;
        private readonly UserManager<Models.Account> _userManager;
        private readonly IUserStore<IdentityUser>? _userStore;
        private readonly IUserEmailStore<IdentityUser>? _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender? _emailSender;

        [BindProperty]
        public required InputModel Input { get; set; }
        public required string? ReturnUrl { get; set; }
        public required IList<AuthenticationScheme> ExternalLogins { get; set; }

        [SetsRequiredMembers]
        public RegisterModel(ILogger<RegisterModel> logger, UserManager<Models.Account> userManager, SignInManager<Models.Account> signInManager, IAccountService accounts)
        {
	        _logger = logger;
	        _userManager = userManager;
	        _signInManager = signInManager;
	        _accounts = accounts;

	        Input = default!;
	        ReturnUrl = default!;
	        ExternalLogins = default!;
        }

        [SetsRequiredMembers]
        protected RegisterModel(
            ILogger<RegisterModel> logger,
            UserManager<Models.Account> userManager,
            SignInManager<Models.Account> signInManager,
            IAccountService accounts,
            IUserStore<IdentityUser> userStore,
            IEmailSender emailSender) : this(logger, userManager, signInManager, accounts)
        {
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _emailSender = emailSender;
        }


        public class InputModel
        {
	        [Required]
	        [Display(Name = "Invite code")]
	        public required string InviteCode { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public required string Email { get; set; }

            [Required]
            [Display(Name = "Username")]
            public required string Handle { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public required string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public required string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var result = await _accounts.RegisterAccount(Input.Email, Input.Handle, Input.Password, Input.InviteCode);
                if (result.Succeeded)
                {
	                var user = await _userManager.FindByEmailAsync(Input.Email);
                    _logger.LogInformation("User created a new account with password");

                    if (_emailSender is not null)
                    {
	                    await SendConfirmationEmail(user, returnUrl);
                    }

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task SendConfirmationEmail(Models.Account user, string returnUrl)
        {
	        var userId = await _userManager.GetUserIdAsync(user);
	        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
	        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
	        var callbackUrl = Url.Page(
		        "/Account/ConfirmEmail",
		        pageHandler: null,
		        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
		        protocol: Request.Scheme);

	        await _emailSender!.SendEmailAsync(Input.Email, "Confirm your email",
		        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        private Models.Account CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Models.Account>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore!;
        }
    }
}
