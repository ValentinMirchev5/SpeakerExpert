using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakerExpert.Business.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";

        // Stored in DB as hash
        public string PasswordHash { get; set; } = "";

        // "Employee" or "Customer" (string, no enums)
        public string Role { get; set; } = "Customer";
    }
}

