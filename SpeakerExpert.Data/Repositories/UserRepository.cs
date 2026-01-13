using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;

namespace SpeakerExpert.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Db _db;

        public UserRepository(Db db)
        {
            _db = db;
        }

        public User? GetByEmail(string email)
        {
            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT Id, Email, PasswordHash, Role FROM Users WHERE Email=@Email",
                conn);

            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new User
            {
                Id = r.GetInt32(0),
                Email = r.GetString(1),
                PasswordHash = r.GetString(2),
                Role = r.GetString(3)
            };
        }

        public User? GetById(int id)
        {
            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT Id, Email, PasswordHash, Role FROM Users WHERE Id=@Id",
                conn);

            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new User
            {
                Id = r.GetInt32(0),
                Email = r.GetString(1),
                PasswordHash = r.GetString(2),
                Role = r.GetString(3)
            };
        }

        public int Create(User user)
        {
            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(
                @"INSERT INTO Users (Email, PasswordHash, Role)
                  OUTPUT INSERTED.Id
                  VALUES (@Email, @PasswordHash, @Role)",
                conn);

            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@Role", user.Role);

            conn.Open();
            return (int)cmd.ExecuteScalar();
        }
    }
}

