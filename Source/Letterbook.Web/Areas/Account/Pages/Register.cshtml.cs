using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Letterbook.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Account.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Models.Account> _signInManager;
        private readonly IAccountService _accounts;
        private readonly UserManager<Models.Account> _userManager;
        private readonly IUserStore<IdentityUser>? _userStore;
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
            if (!ModelState.IsValid) return Page();

            var result = await _accounts.RegisterAccount(Input.Email, Input.Handle, Input.Password, Input.InviteCode);
            if (result.Succeeded)
            {
	            var user = await _userManager.FindByEmailAsync(Input.Email);
	            _logger.LogInformation("User created a new account with password");

	            await _signInManager.SignInAsync(user!, isPersistent: false);
	            return LocalRedirect(returnUrl);
            }
            foreach (var error in result.Errors)
            {
	            ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
