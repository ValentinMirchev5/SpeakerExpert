using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;

namespace SpeakerExpert.Business.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        public ReviewService(IReviewRepository repo) => _repo = repo;

        public List<Review> GetBySpeakerId(int speakerId) => _repo.GetBySpeakerId(speakerId);

        public void Add(Review review)
        {
            // basic validation
            if (review.Rating < 1 || review.Rating > 5)
                throw new System.ArgumentException("Rating must be 1-5.");

            if (string.IsNullOrWhiteSpace(review.Body))
                throw new System.ArgumentException("Review text is required.");

            _repo.Add(review);
        }
    }
}

