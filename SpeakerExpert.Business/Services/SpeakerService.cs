using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;

namespace SpeakerExpert.Business.Services
{
    public class SpeakerService : ISpeakerService
    {
        private readonly ISpeakerRepository _repo;

        public SpeakerService(ISpeakerRepository repo)
        {
            _repo = repo;
        }

        public List<Speaker> GetAll() => _repo.GetAll();

        public Speaker? GetById(int id) => _repo.GetById(id);

        public void Add(Speaker speaker)
        {
            _repo.Add(speaker);
        }


        public void Update(Speaker speaker)
        {

            _repo.Update(speaker);
        }

        public void Delete(int id) => _repo.Delete(id);
    }
}

