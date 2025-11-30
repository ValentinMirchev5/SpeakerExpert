using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Business.Services;
using SpeakerExpert.Models;

namespace SpeakerExpert.Web.Pages.Speakers
{
    public class ManageModel : PageModel
    {
        private readonly SpeakerService _service = new SpeakerService();

        [BindProperty]
        public Speaker NewSpeaker { get; set; } = new();

        [BindProperty]
        public Speaker EditSpeaker { get; set; } = new();

        public List<Speaker> Speakers { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string TypeFilter { get; set; } = "";

        public void OnGet()
        {
            LoadSpeakers();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                LoadSpeakers();
                return Page();
            }

            _service.AddSpeaker(NewSpeaker);
            return RedirectToPage(new { TypeFilter });
        }

        public IActionResult OnPostStartEdit(int id)
        {
            var s = _service.GetSpeakerById(id);
            if (s != null)
                EditSpeaker = s;

            LoadSpeakers();
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            _service.UpdateSpeaker(EditSpeaker);
            return RedirectToPage(new { TypeFilter });
        }

        public IActionResult OnPostDelete(int id)
        {
            _service.DeleteSpeaker(id);
            return RedirectToPage(new { TypeFilter });
        }

        private void LoadSpeakers()
        {
            var all = _service.GetAllSpeakers();

            if (!string.IsNullOrEmpty(TypeFilter))
                Speakers = all.FindAll(s => s.Type.ToLower() == TypeFilter.ToLower());
            else
                Speakers = all;
        }
    }
}
