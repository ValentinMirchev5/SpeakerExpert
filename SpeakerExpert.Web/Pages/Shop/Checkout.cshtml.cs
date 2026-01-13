using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;
using SpeakerExpert.Web.Helpers;
using System.Security.Claims;

namespace SpeakerExpert.Web.Pages.Shop
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private const string CartKey = "cart";

        private readonly IOrderService _orders;

        public CheckoutModel(IOrderService orders)
        {
            _orders = orders;
        }

        public List<CartItem> Cart { get; set; } = new();
        public decimal Total { get; set; }
        public string? Message { get; set; }

        public void OnGet()
        {
            Cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new();
            Total = Cart.Sum(x => x.UnitPrice * x.Quantity);
        }

        public IActionResult OnPost()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new();

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = _orders.Checkout(userId, cart);
            if (!result.ok)
            {
                Message = result.message;
                Cart = cart;
                Total = cart.Sum(x => x.UnitPrice * x.Quantity);
                return Page();
            }

            HttpContext.Session.SetObject(CartKey, new List<CartItem>());

            return RedirectToPage("/Shop/MyOrders");
        }
    }
}

