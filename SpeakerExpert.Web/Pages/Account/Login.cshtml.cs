using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Business.Interfaces;
using System.Security.Claims;

namespace SpeakerExpert.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _auth;

        public LoginModel(IAuthService auth)
        {
            _auth = auth;
        }

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string? Message { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Message = "Invalid user email/password.";
                return Page();
            }

            var user = _auth.Login(Email, Password);

            if (user == null)
            {
                Message = "Invalid email or password.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["SuccessMessage"] = "Login successful!";
            return RedirectToPage("/Shop/Index");

        }
    }
}

