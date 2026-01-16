using Moq;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;
using SpeakerExpert.Business.Services;
using System.Collections.Generic;
using Xunit;

namespace SpeakerExpert.Business.Tests.Services
{
    public class SpeakerServiceTests
    {
        [Fact]
        public void GetAll_ReturnsRepoList()
        {
            // Arrange
            var expected = new List<Speaker>
            {
                new Speaker { Id = 1, Name = "JBL Flip 6", Type = "Portable", Price = 59.99m, Stock = 5 },
                new Speaker { Id = 2, Name = "JBL Charge 5", Type = "Portable", Price = 109.99m, Stock = 2 }
            };

            var repo = new Mock<ISpeakerRepository>();
            repo.Setup(r => r.GetAll()).Returns(expected);

            var service = new SpeakerService(repo.Object);

            // Act
            var result = service.GetAll();

            // Assert
            Assert.Same(expected, result); // same instance returned
            repo.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void GetById_ReturnsSpeakerFromRepo()
        {
            // Arrange
            var speaker = new Speaker { Id = 10, Name = "Test Speaker", Type = "PC", Price = 1.00m, Stock = 1 };

            var repo = new Mock<ISpeakerRepository>();
            repo.Setup(r => r.GetById(10)).Returns(speaker);

            var service = new SpeakerService(repo.Object);

            // Act
            var result = service.GetById(10);

            // Assert
            Assert.Same(speaker, result);
            repo.Verify(r => r.GetById(10), Times.Once);
        }

        [Fact]
        public void Add_CallsRepoAddWithSameSpeaker()
        {
            // Arrange
            var repo = new Mock<ISpeakerRepository>();
            var service = new SpeakerService(repo.Object);

            var newSpeaker = new Speaker
            {
                Id = 0,
                Name = "New Speaker",
                Type = "Portable",
                Price = 25.00m,
                Stock = 3
            };

            // Act
            service.Add(newSpeaker);

            // Assert
            repo.Verify(r => r.Add(newSpeaker), Times.Once);
        }

        [Fact]
        public void Update_CallsRepoUpdateWithSameSpeaker()
        {
            // Arrange
            var repo = new Mock<ISpeakerRepository>();
            var service = new SpeakerService(repo.Object);

            var editSpeaker = new Speaker
            {
                Id = 5,
                Name = "Edited Speaker",
                Type = "Car",
                Price = 50.00m,
                Stock = 10
            };

            // Act
            service.Update(editSpeaker);

            // Assert
            repo.Verify(r => r.Update(editSpeaker), Times.Once);
        }

        [Fact]
        public void Delete_CallsRepoDeleteWithCorrectId()
        {
            // Arrange
            var repo = new Mock<ISpeakerRepository>();
            var service = new SpeakerService(repo.Object);

            // Act
            service.Delete(7);

            // Assert
            repo.Verify(r => r.Delete(7), Times.Once);
        }
    }
}

