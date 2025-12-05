using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Business.Services;
using SpeakerExpert.Models;
using SpeakerExpert.Web.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SpeakerExpert.Web.Pages.Shop
{
    public class IndexModel : PageModel
    {
        private readonly ISpeakerService _service;
        private const string CartSessionKey = "CartItems";

        public IndexModel(ISpeakerService service)
        {
            _service = service;
        }

        public List<Speaker> Speakers { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string TypeFilter { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = "";

        public void OnGet()
        {
            LoadSpeakers();
        }

        public IActionResult OnPostAddToCart(int speakerId, string? typeFilter, string? searchTerm)
        {
            var speaker = _service.GetSpeakerById(speakerId);
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

            return RedirectToPage("/Shop/Index", new { TypeFilter = typeFilter, SearchTerm = searchTerm });
        }

        private void LoadSpeakers()
        {
            var speakers = _service.GetAllSpeakers().AsQueryable();

            if (!string.IsNullOrWhiteSpace(TypeFilter))
            {
                var type = TypeFilter.ToLower();
                speakers = speakers.Where(s => s.Type.ToLower() == type);
            }

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.ToLower();
                speakers = speakers.Where(s =>
                    s.Name.ToLower().Contains(term) ||
                    (s.Description != null && s.Description.ToLower().Contains(term)));
            }

            Speakers = speakers.ToList();
        }
    }
}

