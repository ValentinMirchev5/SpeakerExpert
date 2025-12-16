using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SpeakerExpert.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace SpeakerExpert.Data.Repositories
{
    public class SpeakerRepository : ISpeakerRepository
    {
        private readonly string _connectionString;

        public SpeakerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Speaker> GetAllSpeakers()
        {
            var speakers = new List<Speaker>();

            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT Id, Name, Type, Description, Price, ImageFileName FROM Speakers", conn);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        speakers.Add(new Speaker
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Type = reader.GetString(2),
                            Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Price = reader.GetDecimal(4)
                        });
                    }
                }
            }

            return speakers;
        }

        public Speaker GetSpeakerById(int id)
        {
            Speaker speaker = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(
                    "SELECT Id, Name, Type, Description, Price FROM Speakers WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        speaker = new Speaker
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Type = reader.GetString(2),
                            Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Price = reader.GetDecimal(4)
                        };
                    }
                }
            }

            return speaker;
        }

        public void AddSpeaker(Speaker speaker)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(
    "INSERT INTO Speakers (Name, Type, Description, Price, ImageFileName) VALUES (@Name, @Type, @Description, @Price, @ImageFileName)", conn);

                cmd.Parameters.AddWithValue("@ImageFileName", speaker.ImageFileName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", speaker.Name);
                cmd.Parameters.AddWithValue("@Type", speaker.Type);
                cmd.Parameters.AddWithValue("@Description", speaker.Description ?? "");
                cmd.Parameters.AddWithValue("@Price", speaker.Price);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateSpeaker(Speaker speaker)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(
    "UPDATE Speakers SET Name=@Name, Type=@Type, Description=@Description, Price=@Price, ImageFileName=@ImageFileName WHERE Id=@Id", conn);

                cmd.Parameters.AddWithValue("@ImageFileName", speaker.ImageFileName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", speaker.Name);
                cmd.Parameters.AddWithValue("@Type", speaker.Type);
                cmd.Parameters.AddWithValue("@Description", speaker.Description ?? "");
                cmd.Parameters.AddWithValue("@Price", speaker.Price);
                cmd.Parameters.AddWithValue("@Id", speaker.Id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteSpeaker(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("DELETE FROM Speakers WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}



