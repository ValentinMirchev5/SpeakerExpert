using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SpeakerExpert.Web.Pages.Admin
{
    public class ManageSpeakersModel : PageModel
    {
        private readonly ISpeakerService _service;
        private readonly IWebHostEnvironment _env;

        public ManageSpeakersModel(ISpeakerService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;

        }
        private void Reload()
        {
            Speakers = _service.GetAll();
        }

        public List<Speaker> Speakers { get; set; } = new();

        [BindProperty]
        public Speaker NewSpeaker { get; set; } = new();

        [BindProperty]
        public Speaker EditSpeaker { get; set; } = new();

        [BindProperty]
        public IFormFile? UploadImage { get; set; }

        [BindProperty]
        public bool UploadImageForNew { get; set; } // checkbox

        public void OnGet()
        {
            Speakers = _service.GetAll();
        }

        public IActionResult OnPostAdd()
        {
            // Always reload list for Page() returns
            Speakers = _service.GetAll();

            // If invalid -> show messages
            if (!ModelState.IsValid)
                return Page();

            // optional image
            NewSpeaker.ImageFileName = null;

            if (UploadImageForNew && UploadImage != null && UploadImage.Length > 0)
            {
                NewSpeaker.ImageFileName = SaveImage(UploadImage);
            }

            _service.Add(NewSpeaker);

            // Flash message
            TempData["SuccessMessage"] = "Speaker added successfully!";

            // IMPORTANT: redirect clears ModelState so old errors disappear
            return RedirectToPage();

        }

        public IActionResult OnPostStartEdit(int id)
        {
            var sp = _service.GetById(id);
            if (sp == null) return RedirectToPage();

            EditSpeaker = sp;
            Speakers = _service.GetAll();
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            Speakers = _service.GetAll();

            if (!ModelState.IsValid)
                return Page();

            var existing = _service.GetById(EditSpeaker.Id);
            if (existing == null) return RedirectToPage();

            EditSpeaker.ImageFileName = existing.ImageFileName;

            if (UploadImage != null && UploadImage.Length > 0)
                EditSpeaker.ImageFileName = SaveImage(UploadImage);

            try
            {
                _service.Update(EditSpeaker);

                // Flash message
                TempData["SuccessMessage"] = "Speaker updated successfully!";

                return RedirectToPage();
            }

            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }



        public IActionResult OnPostDelete(int id)
        {
            _service.Delete(id);

            TempData["SuccessMessage"] = "Speaker deleted successfully!";
            return RedirectToPage();
        }


        private string SaveImage(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath, "images", "speakers");
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            file.CopyTo(stream);

            return fileName;
        }
        public IActionResult OnPostRestock(int id, int amount)
        {
            var sp = _service.GetById(id);
            if (sp == null)
            {
                TempData["ErrorMessage"] = "Speaker not found.";
                return RedirectToPage();
            }

            sp.Stock += amount;
            _service.Update(sp);

            TempData["SuccessMessage"] = $"Restocked {sp.Name} (+{amount}). New stock: {sp.Stock}.";
            return RedirectToPage();
        }


    }
}

