using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Models;
using SpeakerExpert.Web.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SpeakerExpert.Web.Pages.Shop
{
    public class CartModel : PageModel
    {
        private const string CartSessionKey = "CartItems";

        public List<CartItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.Total);

        [TempData]
        public string? StatusMessage { get; set; }

        public void OnGet()
        {
            Items = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        public IActionResult OnPostRemove(int speakerId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.SpeakerId == speakerId);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObject(CartSessionKey, cart);
            }

            return RedirectToPage("/Shop/Cart");
        }

        public IActionResult OnPostCheckout()
        {
            HttpContext.Session.Remove(CartSessionKey);
            StatusMessage = "Thank you! Your (demo) order has been placed.";
            return RedirectToPage("/Shop/Cart");
        }
    }
}

