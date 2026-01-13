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
    public class SpeakerRepository : ISpeakerRepository
    {
        private readonly Db _db;

        public SpeakerRepository(Db db)
        {
            _db = db;
        }

        public List<Speaker> GetAll()
        {
            var list = new List<Speaker>();

            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT Id, Name, Type, Description, Price, Stock, ImageFileName FROM Speakers",
                conn);

            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Speaker
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    Type = r.GetString(2),
                    Description = r.IsDBNull(3) ? null : r.GetString(3),
                    Price = r.GetDecimal(4),
                    Stock = r.GetInt32(5),
                    ImageFileName = r.IsDBNull(6) ? null : r.GetString(6)
                });
            }

            return list;
        }

        public Speaker? GetById(int id)
        {
            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT Id, Name, Type, Description, Price, Stock, ImageFileName FROM Speakers WHERE Id=@Id",
                conn);

            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new Speaker
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Type = r.GetString(2),
                Description = r.IsDBNull(3) ? null : r.GetString(3),
                Price = r.GetDecimal(4),
                Stock = r.GetInt32(5),
                ImageFileName = r.IsDBNull(6) ? null : r.GetString(6)
            };
        }

        public void Add(Speaker speaker)
        {
            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(
                @"INSERT INTO Speakers (Name, Type, Description, Price, Stock, ImageFileName)
                  VALUES (@Name, @Type, @Description, @Price, @Stock, @ImageFileName)",
                conn);

            cmd.Parameters.AddWithValue("@Name", speaker.Name);
            cmd.Parameters.AddWithValue("@Type", speaker.Type);
            cmd.Parameters.AddWithValue("@Description", (object?)speaker.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Price", speaker.Price);
            cmd.Parameters.AddWithValue("@Stock", speaker.Stock);
            cmd.Parameters.AddWithValue("@ImageFileName", (object?)speaker.ImageFileName ?? DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Update(Speaker speaker)
        {
            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(
                @"UPDATE Speakers
                  SET Name=@Name, Type=@Type, Description=@Description, Price=@Price, Stock=@Stock, ImageFileName=@ImageFileName
                  WHERE Id=@Id",
                conn);

            cmd.Parameters.AddWithValue("@Id", speaker.Id);
            cmd.Parameters.AddWithValue("@Name", speaker.Name);
            cmd.Parameters.AddWithValue("@Type", speaker.Type);
            cmd.Parameters.AddWithValue("@Description", (object?)speaker.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Price", speaker.Price);
            cmd.Parameters.AddWithValue("@Stock", speaker.Stock);
            cmd.Parameters.AddWithValue("@ImageFileName", (object?)speaker.ImageFileName ?? DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand("DELETE FROM Speakers WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}

