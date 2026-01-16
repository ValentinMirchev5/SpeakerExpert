using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using SpeakerExpert.Business.Security;

namespace SpeakerExpert.Business.Tests.Security
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Hash_WhenCalled_ReturnsNonEmptyStringDifferentFromPassword()
        {
            // Arrange
            var password = "Secret123!";

            // Act
            var hashed = PasswordHasher.Hash(password);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(hashed));
            Assert.NotEqual(password, hashed);
        }

        [Fact]
        public void Verify_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "Secret123!";
            var stored = PasswordHasher.Hash(password);

            // Act
            var ok = PasswordHasher.Verify(password, stored);

            // Assert
            Assert.True(ok);
        }

        [Fact]
        public void Verify_WithWrongPassword_ReturnsFalse()
        {
            // Arrange
            var password = "Secret123!";
            var stored = PasswordHasher.Hash(password);

            // Act
            var ok = PasswordHasher.Verify("WrongPassword!", stored);

            // Assert
            Assert.False(ok);
        }

        [Fact]
        public void Verify_WithInvalidStoredFormat_ReturnsFalse()
        {
            // Arrange
            var password = "Secret123!";
            var invalidStored = "not_a_valid_hash_format";

            // Act
            var ok = PasswordHasher.Verify(password, invalidStored);

            // Assert
            Assert.False(ok);
        }
    }
}

