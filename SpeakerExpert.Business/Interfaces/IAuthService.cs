using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeakerExpert.Business.Domain;

namespace SpeakerExpert.Business.Interfaces
{
    public interface IAuthService
    {
        User? Login(string email, string password);
        (bool ok, string message) Register(string email, string password, string role);
    }
}

