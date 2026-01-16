using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace SpeakerExpert.Business.Domain
{
    public class Speaker
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Type { get; set; } = "";

        public string Description { get; set; } = "";
        [Range(0.01, 999999, ErrorMessage = "Price must be higher than 0.01.")]
        public decimal Price { get; set; }

        [Range(1, 999999, ErrorMessage = "Stock must be at least 1.")]
        public int Stock { get; set; }
        // optional image otherwise uses a placeholder image
        public string? ImageFileName { get; set; }
    }

}

