using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpeakerExpert.Business.Services;
using SpeakerExpert.Models;

namespace SpeakerExpert.Web.Pages.Speakers
{
    public class IndexModel : PageModel
    {
        public List<Speaker> Speakers { get; set; } = new List<Speaker>();

        public void OnGet()
        {
            var service = new SpeakerService();
            Speakers = service.GetAllSpeakers();
        }
    }
}
