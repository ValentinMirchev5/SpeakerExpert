using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;

namespace SpeakerExpert.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orders;
        private readonly ISpeakerRepository _speakers;

        public OrderService(IOrderRepository orders, ISpeakerRepository speakers)
        {
            _orders = orders;
            _speakers = speakers;
        }

        public (bool ok, string message, int orderId) Checkout(int userId, List<CartItem> cart)
        {
            if (cart == null || cart.Count == 0)
                return (false, "Your cart is empty.", 0);

            // Validate and calculate using DB prices + stock
            decimal total = 0;

            foreach (var item in cart)
            {
                var sp = _speakers.GetById(item.SpeakerId);
                if (sp == null)
                    return (false, $"Speaker not found (ID {item.SpeakerId}).", 0);

                if (sp.Stock < item.Quantity)
                    return (false, $"Not enough stock for {sp.Name}. Available: {sp.Stock}.", 0);

                total += sp.Price * item.Quantity;
            }

            // Create order
            int orderId = _orders.CreateOrder(userId, total);

            // Add items + decrease stock
            foreach (var item in cart)
            {
                var sp = _speakers.GetById(item.SpeakerId)!;

                _orders.AddOrderItem(orderId, new OrderItem
                {
                    OrderId = orderId,
                    SpeakerId = sp.Id,
                    SpeakerName = sp.Name,
                    UnitPrice = sp.Price,
                    Quantity = item.Quantity
                });

                sp.Stock -= item.Quantity;
                _speakers.Update(sp);
            }

            return (true, "Checkout complete!", orderId);
        }

        public List<Order> GetMyOrders(int userId)
        {
            return _orders.GetOrdersByUser(userId);
        }
    }
}

