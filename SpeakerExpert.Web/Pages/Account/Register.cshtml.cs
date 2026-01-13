using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Business.Interfaces;

namespace SpeakerExpert.Web.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthService _auth;

        public RegisterModel(IAuthService auth)
        {
            _auth = auth;
        }

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        [BindProperty]
        public string Role { get; set; } = "Customer"; // "Customer" or "Employee"

        public string? Message { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            var result = _auth.Register(Email, Password, Role);
            if (!result.ok)
            {
                Message = result.message;
                return Page();
            }

            return RedirectToPage("/Account/Login");
        }
    }
}

