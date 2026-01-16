using Moq;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;
using SpeakerExpert.Business.Services;
using SpeakerExpert.Business.Security;
using Xunit;

namespace SpeakerExpert.Business.Tests.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public void Login_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var usersRepo = new Mock<IUserRepository>();
            usersRepo.Setup(r => r.GetByEmail("missing@test.com")).Returns((User?)null);

            var service = new AuthService(usersRepo.Object);

            // Act
            var result = service.Login("missing@test.com", "anyPassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Login_WhenPasswordIsWrong_ReturnsNull()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "user@test.com",
                PasswordHash = PasswordHasher.Hash("correctPassword"),
                Role = "Customer"
            };

            var usersRepo = new Mock<IUserRepository>();
            usersRepo.Setup(r => r.GetByEmail("user@test.com")).Returns(user);

            var service = new AuthService(usersRepo.Object);

            // Act
            var result = service.Login("user@test.com", "wrongPassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Login_WhenCredentialsAreValid_ReturnsUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "user@test.com",
                PasswordHash = PasswordHasher.Hash("correctPassword"),
                Role = "Customer"
            };

            var usersRepo = new Mock<IUserRepository>();
            usersRepo.Setup(r => r.GetByEmail("user@test.com")).Returns(user);

            var service = new AuthService(usersRepo.Object);

            // Act
            var result = service.Login("user@test.com", "correctPassword");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user@test.com", result!.Email);
        }

        [Fact]
        public void Register_WhenEmailIsInvalid_ReturnsFail()
        {
            // Arrange
            var usersRepo = new Mock<IUserRepository>();
            var service = new AuthService(usersRepo.Object);

            // Act
            var (ok, message) = service.Register("notAnEmail", "123456", "Customer");

            // Assert
            Assert.False(ok);
            Assert.Equal("Please enter a valid email.", message);
        }

        [Fact]
        public void Register_WhenPasswordTooShort_ReturnsFail()
        {
            // Arrange
            var usersRepo = new Mock<IUserRepository>();
            var service = new AuthService(usersRepo.Object);

            // Act
            var (ok, message) = service.Register("user@test.com", "123", "Customer");

            // Assert
            Assert.False(ok);
            Assert.Equal("Password must be at least 6 characters.", message);
        }

        [Fact]
        public void Register_WhenRoleIsInvalid_ReturnsFail()
        {
            // Arrange
            var usersRepo = new Mock<IUserRepository>();
            var service = new AuthService(usersRepo.Object);

            // Act
            var (ok, message) = service.Register("user@test.com", "123456", "Admin");

            // Assert
            Assert.False(ok);
            Assert.Equal("Invalid role.", message);
        }

        [Fact]
        public void Register_WhenEmailAlreadyExists_ReturnsFail()
        {
            // Arrange
            var existing = new User { Id = 2, Email = "user@test.com", PasswordHash = "x", Role = "Customer" };

            var usersRepo = new Mock<IUserRepository>();
            usersRepo.Setup(r => r.GetByEmail("user@test.com")).Returns(existing);

            var service = new AuthService(usersRepo.Object);

            // Act
            var (ok, message) = service.Register("user@test.com", "123456", "Customer");

            // Assert
            Assert.False(ok);
            Assert.Equal("Email is already registered.", message);
        }

        [Fact]
        public void Register_WhenValid_CreatesUserAndReturnsSuccess()
        {
            // Arrange
            var usersRepo = new Mock<IUserRepository>();
            usersRepo.Setup(r => r.GetByEmail("new@test.com")).Returns((User?)null);

            var service = new AuthService(usersRepo.Object);

            // Act
            var (ok, message) = service.Register("new@test.com", "123456", "Customer");

            // Assert
            Assert.True(ok);
            Assert.Equal("Account created.", message);

            usersRepo.Verify(r => r.Create(It.Is<User>(u =>
                u.Email == "new@test.com" &&
                u.Role == "Customer" &&
                !string.IsNullOrWhiteSpace(u.PasswordHash)
            )), Times.Once);
        }
    }
}

