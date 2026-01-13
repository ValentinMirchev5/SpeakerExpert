using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakerExpert.Business.Domain
{
    public class CartItem
    {
        public int SpeakerId { get; set; }
        public string SpeakerName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}


