using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeakerExpert.Business.Domain;

namespace SpeakerExpert.Business.Interfaces
{
    public interface IOrderService
    {
        (bool ok, string message, int orderId) Checkout(int userId, List<CartItem> cart);
        List<Order> GetMyOrders(int userId);
    }
}

