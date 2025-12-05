using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Business.Services;
using SpeakerExpert.Models;
using SpeakerExpert.Web.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SpeakerExpert.Web.Pages.Shop
{
    public class DetailsModel : PageModel
    {
        private readonly ISpeakerService _service;
        private const string CartSessionKey = "CartItems";

        public DetailsModel(ISpeakerService service)
        {
            _service = service;
        }

        public Speaker? Speaker { get; set; }

        public IActionResult OnGet(int id)
        {
            Speaker = _service.GetSpeakerById(id);
            if (Speaker == null)
                return RedirectToPage("/Shop/Index");

            return Page();
        }

        public IActionResult OnPostAddToCart(int id)
        {
            var speaker = _service.GetSpeakerById(id);
            if (speaker != null)
            {
                var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

                var existing = cart.FirstOrDefault(c => c.SpeakerId == speaker.Id);
                if (existing == null)
                {
                    cart.Add(new CartItem
                    {
                        SpeakerId = speaker.Id,
                        Name = speaker.Name,
                        Type = speaker.Type,
                        Price = speaker.Price,
                        Quantity = 1
                    });
                }
                else
                {
                    existing.Quantity += 1;
                }

                HttpContext.Session.SetObject(CartSessionKey, cart);
            }

            return RedirectToPage("/Shop/Cart");
        }
    }
}

