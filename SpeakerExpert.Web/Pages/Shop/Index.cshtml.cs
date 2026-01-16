using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;
using SpeakerExpert.Web.Helpers;

namespace SpeakerExpert.Web.Pages.Shop
{
    public class IndexModel : PageModel
    {
        private readonly ISpeakerService _speakers;
        private const string CartKey = "cart";

        public IndexModel(ISpeakerService speakers)
        {
            _speakers = speakers;

        }
        public IActionResult OnPostAddToCart(int id)
        {
            var sp = _speakers.GetById(id);
            if (sp == null) return RedirectToPage();

            if (sp.Stock <= 0)
            {
                TempData["CartError"] = "This item is out of stock and cannot be added to the cart.";
                return RedirectToPage(new { Search = Search, Type = Type, MinPrice = MinPrice, MaxPrice = MaxPrice, Sort = Sort });
            }

            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.SpeakerId == id);
            if (item == null)
            {
                cart.Add(new CartItem
                {
                    SpeakerId = id,
                    SpeakerName = sp.Name,
                    UnitPrice = sp.Price,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity += 1;
            }

            HttpContext.Session.SetObject(CartKey, cart);

            return RedirectToPage(new { Search = Search, Type = Type, MinPrice = MinPrice, MaxPrice = MaxPrice, Sort = Sort });
        }


        public List<Speaker> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Type { get; set; } // "Car", "Portable", "PC"


        [BindProperty(SupportsGet = true)] public decimal? MinPrice { get; set; }
        [BindProperty(SupportsGet = true)] public decimal? MaxPrice { get; set; }
        [BindProperty(SupportsGet = true)] public bool InStockOnly { get; set; }
        [BindProperty(SupportsGet = true)] public string Sort { get; set; } = "name_asc";

        public void OnGet()
        {
            var all = _speakers.GetAll();

            // Search (name/description)
            if (!string.IsNullOrWhiteSpace(Search))
            {
                var s = Search.Trim().ToLower();
                all = all.Where(x =>
                    (x.Name ?? "").ToLower().Contains(s) ||
                    (x.Description ?? "").ToLower().Contains(s)
                ).ToList();
            }

            // Type
            if (!string.IsNullOrWhiteSpace(Type) && Type != "All")
            {
                all = all.Where(x => x.Type == Type).ToList();
            }

            // Price range
            if (MinPrice.HasValue)
                all = all.Where(x => x.Price >= MinPrice.Value).ToList();

            if (MaxPrice.HasValue)
                all = all.Where(x => x.Price <= MaxPrice.Value).ToList();

            // Stock only
            if (InStockOnly)
                all = all.Where(x => x.Stock > 0).ToList();

            // Sort
            all = Sort switch
            {
                "name_desc" => all.OrderByDescending(x => x.Name).ToList(),
                "price_asc" => all.OrderBy(x => x.Price).ToList(),
                "price_desc" => all.OrderByDescending(x => x.Price).ToList(),
                "stock_desc" => all.OrderByDescending(x => x.Stock).ToList(),
                _ => all.OrderBy(x => x.Name).ToList(), // name_asc default
            };

            Items = all;
        }



        public string GetImageFile(Speaker sp)
        {
            return string.IsNullOrWhiteSpace(sp.ImageFileName) ? "placeholder.png" : sp.ImageFileName!;
        }
    }
}

