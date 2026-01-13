using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeakerExpert.Business.Domain;

namespace SpeakerExpert.Business.Interfaces
{
    public interface IUserRepository
    {
        User? GetByEmail(string email);
        User? GetById(int id);
        int Create(User user); // returns new user id
    }
}

