using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SpeakerExpert.Business.Domain;

public interface IReviewRepository
{
    List<Review> GetBySpeakerId(int speakerId);
    void Add(Review review);
}

