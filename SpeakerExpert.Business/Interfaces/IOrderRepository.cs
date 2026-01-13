using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeakerExpert.Business.Domain;

namespace SpeakerExpert.Business.Interfaces
{
    public interface IOrderRepository
    {
        int CreateOrder(int userId, decimal total);
        void AddOrderItem(int orderId, OrderItem item);
        List<Order> GetOrdersByUser(int userId);
    }
}

