using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SpeakerExpert.Web.Pages.Shop
{
    public class DetailsModel : PageModel
    {
        private readonly ISpeakerService _speakers;
        private readonly IReviewService _reviews;

        public DetailsModel(ISpeakerService speakers, IReviewService reviews)
        {
            _speakers = speakers;
            _reviews = reviews;
        }

        public Speaker? Speaker { get; set; }

        public List<Review> Reviews { get; set; } = new();

        [BindProperty]
        public NewReviewInput NewReview { get; set; } = new();

        public class NewReviewInput
        {
            [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
            public int Rating { get; set; } = 5;

            [StringLength(80)]
            public string? Title { get; set; }

            [Required(ErrorMessage = "Review text is required.")]
            [StringLength(1000)]
            public string Body { get; set; } = "";
        }

        public IActionResult OnGet(int id)
        {
            Speaker = _speakers.GetById(id);
            if (Speaker == null) return RedirectToPage("/Shop/Index");

            Reviews = _reviews.GetBySpeakerId(id);

            return Page();
        }

        public IActionResult OnPostAddReview(int id)
        {
            // re-load speaker + reviews so page can re-render if validation fails
            Speaker = _speakers.GetById(id);
            if (Speaker == null) return RedirectToPage("/Shop/Index");

            Reviews = _reviews.GetBySpeakerId(id);

            if (!User.Identity?.IsAuthenticated ?? true)
                return Challenge(); // sends to login

            if (!ModelState.IsValid)
                return Page();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Forbid();

            _reviews.Add(new Review
            {
                SpeakerId = id,
                UserId = userId,
                Rating = NewReview.Rating,
                Title = NewReview.Title,
                Body = NewReview.Body
            });

            return RedirectToPage(new { id });
        }

        public string Img =>
            string.IsNullOrWhiteSpace(Speaker?.ImageFileName) ? "placeholder.png" : Speaker!.ImageFileName!;
    }
}


