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

        public decimal Price { get; set; }

        public int Stock { get; set; }
        // optional image otherwise uses a placeholder image
        public string? ImageFileName { get; set; }
    }

}

