using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SpeakerExpert.Business.Domain;

namespace SpeakerExpert.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly Db _db;
        public ReviewRepository(Db db) => _db = db;

        public List<Review> GetBySpeakerId(int speakerId)
        {
            var list = new List<Review>();

            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(@"
                SELECT r.Id, r.SpeakerId, r.UserId, r.Rating, r.Title, r.Body, r.CreatedAt,
                       u.Email AS UserEmail
                FROM Reviews r
                JOIN Users u ON u.Id = r.UserId
                WHERE r.SpeakerId = @SpeakerId
                ORDER BY r.CreatedAt DESC;", conn);

            cmd.Parameters.AddWithValue("@SpeakerId", speakerId);

            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Review
                {
                    Id = (int)rd["Id"],
                    SpeakerId = (int)rd["SpeakerId"],
                    UserId = (int)rd["UserId"],
                    Rating = (int)rd["Rating"],
                    Title = rd["Title"] as string,
                    Body = (string)rd["Body"],
                    CreatedAt = (System.DateTime)rd["CreatedAt"],
                    UserEmail = rd["UserEmail"] as string
                });
            }
            return list;
        }

        public void Add(Review review)
        {
            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO Reviews (SpeakerId, UserId, Rating, Title, Body)
                VALUES (@SpeakerId, @UserId, @Rating, @Title, @Body);", conn);

            cmd.Parameters.AddWithValue("@SpeakerId", review.SpeakerId);
            cmd.Parameters.AddWithValue("@UserId", review.UserId);
            cmd.Parameters.AddWithValue("@Rating", review.Rating);
            cmd.Parameters.AddWithValue("@Title", (object?)review.Title ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@Body", review.Body);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}

