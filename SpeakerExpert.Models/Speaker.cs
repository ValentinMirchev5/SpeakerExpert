using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SpeakerExpert.Models
{
    public class Speaker
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = ""; // Car / Portable / PC

        [StringLength(255)]
        public string Description { get; set; }

        [Range(0.01, 100000)]
        public decimal Price { get; set; }
        public string ImageFileName { get; set; }

    }
}

