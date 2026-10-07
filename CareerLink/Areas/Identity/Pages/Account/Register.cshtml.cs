using CareerLink.Data;
using CareerLink.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace CareerLink.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;
    private readonly ApplicationDbContext _context;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public string AccountType { get; set; } = "JobSeeker";

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(
            100,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(
            "Password",
            ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        ExternalLogins =
            (await _signInManager.GetExternalAuthenticationSchemesAsync())
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        ExternalLogins =
            (await _signInManager.GetExternalAuthenticationSchemesAsync())
            .ToList();

        // Only these two account types are allowed through normal registration.
        if (AccountType != "JobSeeker" &&
            AccountType != "Recruiter")
        {
            ModelState.AddModelError(
                nameof(AccountType),
                "Please select a valid account type.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = CreateUser();

        await _userStore.SetUserNameAsync(
            user,
            Input.Email,
            CancellationToken.None);

        await _emailStore.SetEmailAsync(
            user,
            Input.Email,
            CancellationToken.None);

        var result = await _userManager.CreateAsync(
            user,
            Input.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return Page();
        }

        _logger.LogInformation(
            "User created a new account with password.");

        var roleName = AccountType;

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            roleName);

        if (!roleResult.Succeeded)
        {
            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            await _userManager.DeleteAsync(user);

            return Page();
        }

        if (AccountType == "JobSeeker")
        {
            var existingProfile = await _context.JobSeekers
                .AnyAsync(x => x.ApplicationUserId == user.Id);

            if (!existingProfile)
            {
                _context.JobSeekers.Add(new JobSeeker
                {
                    ApplicationUserId = user.Id
                });
            }
        }
        else if (AccountType == "Recruiter")
        {
            var existingProfile = await _context.Recruiters
                .AnyAsync(x => x.ApplicationUserId == user.Id);

            if (!existingProfile)
            {
                _context.Recruiters.Add(new Recruiter
                {
                    ApplicationUserId = user.Id
                });
            }
        }

        await _context.SaveChangesAsync();

        var userId = await _userManager.GetUserIdAsync(user);

        var code =
            await _userManager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(code));

        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new
            {
                area = "Identity",
                userId,
                code,
                returnUrl
            },
            protocol: Request.Scheme)!;

        await _emailSender.SendEmailAsync(
            Input.Email,
            "Confirm your email",
            $"Please confirm your account by " +
            $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>" +
            $"clicking here</a>.");

        if (_userManager.Options.SignIn.RequireConfirmedAccount)
        {
            return RedirectToPage(
                "RegisterConfirmation",
                new
                {
                    email = Input.Email,
                    returnUrl
                });
        }

        await _signInManager.SignInAsync(
            user,
            isPersistent: false);

        return LocalRedirect(returnUrl);
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException(
                $"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class " +
                $"and has a parameterless constructor, or alternatively " +
                $"override the register page in " +
                $"/Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException(
                "The default UI requires a user store with email support.");
        }

        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
}