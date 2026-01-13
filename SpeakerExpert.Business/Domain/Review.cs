using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SpeakerExpert.Business.Domain
{
    public class Review
    {
        public int Id { get; set; }
        public int SpeakerId { get; set; }
        public int UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(80)]
        public string? Title { get; set; }

        [Required, StringLength(1000)]
        public string Body { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        // optional for UI display
        public string? UserEmail { get; set; }
    }
}
