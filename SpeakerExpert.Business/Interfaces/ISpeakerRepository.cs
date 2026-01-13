using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeakerExpert.Business.Domain;

namespace SpeakerExpert.Business.Interfaces
{
    public interface ISpeakerRepository
    {
        List<Speaker> GetAll();
        Speaker? GetById(int id);

        void Add(Speaker speaker);
        void Update(Speaker speaker);
        void Delete(int id);
    }
}

