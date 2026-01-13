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
    public class OrderRepository : IOrderRepository
    {
        private readonly Db _db;

        public OrderRepository(Db db)
        {
            _db = db;
        }

        public int CreateOrder(int userId, decimal total)
        {
            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(@"
INSERT INTO Orders (UserId, Total)
OUTPUT INSERTED.Id
VALUES (@UserId, @Total);", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Total", total);

            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void AddOrderItem(int orderId, OrderItem item)
        {
            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(@"
INSERT INTO OrderItems (OrderId, SpeakerId, SpeakerName, UnitPrice, Quantity)
VALUES (@OrderId, @SpeakerId, @SpeakerName, @UnitPrice, @Quantity);", conn);

            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@SpeakerId", item.SpeakerId);
            cmd.Parameters.AddWithValue("@SpeakerName", item.SpeakerName);
            cmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Order> GetOrdersByUser(int userId)
        {
            var orders = new List<Order>();

            using (var conn = new SqlConnection(_db.ConnectionString))
            using (var cmd = new SqlCommand(@"
SELECT Id, UserId, CreatedAt, Total
FROM Orders
WHERE UserId = @UserId
ORDER BY CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();

                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    orders.Add(new Order
                    {
                        Id = r.GetInt32(0),
                        UserId = r.GetInt32(1),
                        CreatedAt = r.GetDateTime(2),
                        Total = r.GetDecimal(3)
                    });
                }
            }

            foreach (var o in orders)
                o.Items = GetItems(o.Id);

            return orders;
        }

        private List<OrderItem> GetItems(int orderId)
        {
            var items = new List<OrderItem>();

            using var conn = new SqlConnection(_db.ConnectionString);
            using var cmd = new SqlCommand(@"
SELECT Id, OrderId, SpeakerId, SpeakerName, UnitPrice, Quantity
FROM OrderItems
WHERE OrderId = @OrderId;", conn);

            cmd.Parameters.AddWithValue("@OrderId", orderId);

            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                items.Add(new OrderItem
                {
                    Id = r.GetInt32(0),
                    OrderId = r.GetInt32(1),
                    SpeakerId = r.GetInt32(2),
                    SpeakerName = r.GetString(3),
                    UnitPrice = r.GetDecimal(4),
                    Quantity = r.GetInt32(5)
                });
            }

            return items;
        }
    }
}

