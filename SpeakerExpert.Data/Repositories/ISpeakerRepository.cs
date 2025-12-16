using SpeakerExpert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakerExpert.Data.Repositories
{
    public interface ISpeakerRepository
    {
        List<Speaker> GetAllSpeakers();
        Speaker GetSpeakerById(int id);
        void AddSpeaker(Speaker speaker);
        void UpdateSpeaker(Speaker speaker);
        void DeleteSpeaker(int id);
    }
}
