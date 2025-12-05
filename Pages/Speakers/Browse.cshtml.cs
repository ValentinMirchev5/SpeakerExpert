using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Business.Services;
using SpeakerExpert.Models;
using System.Collections.Generic;
using System.Linq;

namespace SpeakerExpert.Web.Pages.Speakers
{
    public class BrowseModel : PageModel
    {
        private readonly ISpeakerService _service;

        public BrowseModel(ISpeakerService service)
        {
            _service = service;
        }

        public List<Speaker> Speakers { get; set; } = new();

        // Filter by type 
        [BindProperty(SupportsGet = true)]
        public string TypeFilter { get; set; } = "";

        // Search in name + description
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = "";

        // Sorting
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "Name";  // Name, Type, Price

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "asc"; // asc or desc

        public void OnGet()
        {
            LoadSpeakers();
        }

        public IActionResult OnPostDelete(int id, string? typeFilter, string? searchTerm, string? sortBy, string? sortDirection)
        {
            _service.DeleteSpeaker(id);

            // Keep same filters and sorting after delete
            return RedirectToPage("/Speakers/Browse", new
            {
                TypeFilter = typeFilter,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortDirection = sortDirection
            });
        }

        private void LoadSpeakers()
        {
            var speakers = _service.GetAllSpeakers().AsQueryable();

            // Type filter
            if (!string.IsNullOrWhiteSpace(TypeFilter))
            {
                var type = TypeFilter.ToLower();
                speakers = speakers.Where(s => s.Type.ToLower() == type);
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.ToLower();
                speakers = speakers.Where(s =>
                    s.Name.ToLower().Contains(term) ||
                    (s.Description != null && s.Description.ToLower().Contains(term)));
            }

            // Sorting
            bool asc = SortDirection.ToLower() != "desc";

            switch (SortBy?.ToLower())
            {
                case "type":
                    speakers = asc
                        ? speakers.OrderBy(s => s.Type).ThenBy(s => s.Name)
                        : speakers.OrderByDescending(s => s.Type).ThenByDescending(s => s.Name);
                    break;

                case "price":
                    speakers = asc
                        ? speakers.OrderBy(s => s.Price)
                        : speakers.OrderByDescending(s => s.Price);
                    break;

                default: // Name
                    speakers = asc
                        ? speakers.OrderBy(s => s.Name)
                        : speakers.OrderByDescending(s => s.Name);
                    break;
            }

            Speakers = speakers.ToList();
        }
    }
}

