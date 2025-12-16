using SpeakerExpert.Data.Repositories;
using SpeakerExpert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakerExpert.Business.Services
{
    public class SpeakerService : ISpeakerService
    {
        private readonly ISpeakerRepository _repository;

        public SpeakerService(ISpeakerRepository repository)
        {
            _repository = repository;
        }

        public List<Speaker> GetAllSpeakers()
        {
            return _repository.GetAllSpeakers();
        }

        public Speaker GetSpeakerById(int id)
        {
            return _repository.GetSpeakerById(id);
        }

        public void AddSpeaker(Speaker speaker)
        {
            _repository.AddSpeaker(speaker);
        }

        public void UpdateSpeaker(Speaker speaker)
        {
            _repository.UpdateSpeaker(speaker);
        }

        public void DeleteSpeaker(int id)
        {
            _repository.DeleteSpeaker(id);
        }
    }
}
