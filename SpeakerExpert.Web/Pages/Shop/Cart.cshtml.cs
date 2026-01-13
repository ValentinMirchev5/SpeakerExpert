using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;
using SpeakerExpert.Web.Helpers;

namespace SpeakerExpert.Web.Pages.Shop
{
    public class CartModel : PageModel
    {
        private const string CartKey = "cart";

        private readonly ICartService _cartService;

        public CartModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        public List<CartItem> Cart { get; set; } = new();
        public decimal Total { get; set; }

        public void OnGet()
        {
            Cart = _cartService.GetCart(HttpContext.Session.GetObject<List<CartItem>>(CartKey));
            Total = _cartService.Total(Cart);
        }

        public IActionResult OnPostUpdate(int speakerId, int quantity)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey);
            cart = _cartService.UpdateQuantity(cart, speakerId, quantity);
            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToPage();
        }

        public IActionResult OnPostRemove(int speakerId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey);
            cart = _cartService.Remove(cart, speakerId);
            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToPage();
        }

        public IActionResult OnPostClear()
        {
            HttpContext.Session.SetObject(CartKey, _cartService.Clear());
            return RedirectToPage();
        }
    }
}

