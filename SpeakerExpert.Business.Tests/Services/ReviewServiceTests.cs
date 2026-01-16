using System;
using System.Collections.Generic;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;
using SpeakerExpert.Business.Services;
using Xunit;

namespace SpeakerExpert.Business.Tests
{
    public class ReviewServiceTests
    {
        // Simple fake repo for unit tests (Business layer only)
        private class FakeReviewRepository : IReviewRepository
        {
            public List<Review> ReviewsBySpeakerReturn { get; set; } = new();
            public Review? AddedReview { get; private set; }

            public List<Review> GetBySpeakerId(int speakerId)
            {
                return ReviewsBySpeakerReturn;
            }

            public void Add(Review review)
            {
                AddedReview = review;
            }
        }

        [Fact]
        public void GetBySpeakerId_ReturnsListFromRepository()
        {
            // Arrange
            var repo = new FakeReviewRepository
            {
                ReviewsBySpeakerReturn = new List<Review>
                {
                    new Review { Id = 1, SpeakerId = 10, Rating = 5, Body = "Great" },
                    new Review { Id = 2, SpeakerId = 10, Rating = 4, Body = "Good" }
                }
            };

            var service = new ReviewService(repo);

            // Act
            var result = service.GetBySpeakerId(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(2, result[1].Id);
        }

        [Fact]
        public void Add_WithValidReview_CallsRepositoryAdd()
        {
            // Arrange
            var repo = new FakeReviewRepository();
            var service = new ReviewService(repo);

            var review = new Review
            {
                SpeakerId = 1,
                UserId = 123,
                Rating = 5,
                Title = "Nice",
                Body = "Solid speaker",
                CreatedAt = DateTime.Now
            };

            // Act
            service.Add(review);

            // Assert
            Assert.NotNull(repo.AddedReview);
            Assert.Same(review, repo.AddedReview); // confirms the same object was passed into repo.Add
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void Add_WithRatingOutOfRange_ThrowsArgumentException(int rating)
        {
            // Arrange
            var repo = new FakeReviewRepository();
            var service = new ReviewService(repo);

            var review = new Review
            {
                SpeakerId = 1,
                UserId = 123,
                Rating = rating,
                Body = "Text"
            };

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => service.Add(review));
            Assert.Contains("Rating", ex.Message);
            Assert.Null(repo.AddedReview); // repo.Add should NOT have been called
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Add_WithMissingBody_ThrowsArgumentException(string? body)
        {
            // Arrange
            var repo = new FakeReviewRepository();
            var service = new ReviewService(repo);

            var review = new Review
            {
                SpeakerId = 1,
                UserId = 123,
                Rating = 5,
                Body = body ?? ""
            };

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => service.Add(review));
            Assert.Contains("Review", ex.Message);
            Assert.Null(repo.AddedReview);
        }
    }
}

