using SpeakerExpert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakerExpert.Business.Services
{
    //Interface for CRUD
    public interface ISpeakerService
    {
        List<Speaker> GetAllSpeakers();
        Speaker GetSpeakerById(int id);
        void AddSpeaker(Speaker speaker);
        void UpdateSpeaker(Speaker speaker);
        void DeleteSpeaker(int id);
    }
}
