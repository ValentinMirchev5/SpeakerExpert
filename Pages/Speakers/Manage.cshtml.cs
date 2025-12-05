using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Business.Services;
using SpeakerExpert.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;


namespace SpeakerExpert.Web.Pages.Speakers
{
    public class ManageModel : PageModel
    {

        private readonly ISpeakerService _service;

        public ManageModel(ISpeakerService service)
        {
            _service = service;
        }

        [BindProperty]
        public Speaker NewSpeaker { get; set; } = new();

        [BindProperty]
        public Speaker EditSpeaker { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public void OnGet(int? id)
        {
            if (id.HasValue)
            {
                var s = _service.GetSpeakerById(id.Value);
                if (s != null)
                    EditSpeaker = s;
            }
        }

        [BindProperty]
        public IFormFile? UploadImage { get; set; }
        public bool UseImageForNew { get; set; }
        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            // Default: use placeholder image
            NewSpeaker.ImageFileName = "placeholder.png";

            // Only try to save an image if the checkbox is ticked AND a file is chosen
            if (UseImageForNew && UploadImage != null && UploadImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "speakers");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(UploadImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    UploadImage.CopyTo(stream);
                }

                NewSpeaker.ImageFileName = fileName;
            }

            _service.AddSpeaker(NewSpeaker);
            StatusMessage = "Speaker added successfully.";
            return RedirectToPage("/Speakers/Manage");
        }


        public IActionResult OnPostUpdate()
        {
            if (!ModelState.IsValid)
                return Page();

            var existing = _service.GetSpeakerById(EditSpeaker.Id);
            if (existing != null)
            {
                EditSpeaker.ImageFileName = existing.ImageFileName;
            }

            if (UploadImage != null && UploadImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "speakers");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(UploadImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    UploadImage.CopyTo(stream);
                }

                EditSpeaker.ImageFileName = fileName;
            }

            _service.UpdateSpeaker(EditSpeaker);
            StatusMessage = "Speaker updated successfully.";
            return RedirectToPage("/Speakers/Browse");
        }

    }
}
