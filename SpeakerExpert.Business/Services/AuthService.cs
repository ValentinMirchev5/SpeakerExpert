using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;
using SpeakerExpert.Business.Security;

namespace SpeakerExpert.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;

        public AuthService(IUserRepository users)
        {
            _users = users;
        }

        public User? Login(string email, string password)
        {
            var user = _users.GetByEmail(email);
            if (user == null) return null;

            return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
        }

        public (bool ok, string message) Register(string email, string password, string role)
        {
            email = email.Trim();

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return (false, "Please enter a valid email.");

            if (password.Length < 6)
                return (false, "Password must be at least 6 characters.");

            if (role != "Employee" && role != "Customer")
                return (false, "Invalid role.");

            if (_users.GetByEmail(email) != null)
                return (false, "Email is already registered.");

            var newUser = new User
            {
                Email = email,
                PasswordHash = PasswordHasher.Hash(password),
                Role = role
            };

            _users.Create(newUser);
            return (true, "Account created.");
        }
    }
}

