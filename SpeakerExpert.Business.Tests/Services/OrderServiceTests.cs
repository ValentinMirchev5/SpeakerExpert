using System;
using System.Collections.Generic;
using Xunit;

using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;
using SpeakerExpert.Business.Services;

namespace SpeakerExpert.Business.Tests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public void Checkout_WhenCartIsEmpty_ReturnsFalseWithMessage()
        {
            // Arrange
            var ordersRepo = new FakeOrderRepository();
            var speakersRepo = new FakeSpeakerRepository();
            var service = new OrderService(ordersRepo, speakersRepo);

            var cart = new List<CartItem>(); // empty

            // Act
            var result = service.Checkout(userId: 1, cart);

            // Assert
            Assert.False(result.ok);
            Assert.Equal("Your cart is empty.", result.message);
            Assert.Equal(0, result.orderId);
            Assert.Equal(0, ordersRepo.CreateOrderCallCount);
        }

        [Fact]
        public void Checkout_WhenSpeakerNotFound_ReturnsFalse()
        {
            // Arrange
            var ordersRepo = new FakeOrderRepository();
            var speakersRepo = new FakeSpeakerRepository(); // no speakers added
            var service = new OrderService(ordersRepo, speakersRepo);

            var cart = new List<CartItem>
            {
                new CartItem { SpeakerId = 99, SpeakerName = "Doesn't matter", UnitPrice = 1m, Quantity = 1 }
            };

            // Act
            var result = service.Checkout(userId: 1, cart);

            // Assert
            Assert.False(result.ok);
            Assert.Contains("Speaker not found", result.message);
            Assert.Equal(0, result.orderId);
            Assert.Equal(0, ordersRepo.CreateOrderCallCount);
        }

        [Fact]
        public void Checkout_WhenNotEnoughStock_ReturnsFalse()
        {
            // Arrange
            var ordersRepo = new FakeOrderRepository();
            var speakersRepo = new FakeSpeakerRepository();
            speakersRepo.Seed(new Speaker { Id = 10, Name = "JBL Flip 6", Price = 100m, Stock = 1 });

            var service = new OrderService(ordersRepo, speakersRepo);

            var cart = new List<CartItem>
            {
                new CartItem { SpeakerId = 10, SpeakerName = "Ignored", UnitPrice = 1m, Quantity = 2 }
            };

            // Act
            var result = service.Checkout(userId: 1, cart);

            // Assert
            Assert.False(result.ok);
            Assert.Contains("Not enough stock", result.message);
            Assert.Equal(0, result.orderId);
            Assert.Equal(0, ordersRepo.CreateOrderCallCount);
        }

        [Fact]
        public void Checkout_WhenValid_CreatesOrder_AddsItems_AndReducesStock()
        {
            // Arrange
            var ordersRepo = new FakeOrderRepository();
            var speakersRepo = new FakeSpeakerRepository();

            // DB speakers (important: price comes from DB, not cart)
            speakersRepo.Seed(new Speaker { Id = 1, Name = "A", Price = 10m, Stock = 10 });
            speakersRepo.Seed(new Speaker { Id = 2, Name = "B", Price = 5m, Stock = 3 });

            var service = new OrderService(ordersRepo, speakersRepo);

            var cart = new List<CartItem>
            {
                new CartItem { SpeakerId = 1, SpeakerName = "WRONG NAME OK", UnitPrice = 999m, Quantity = 2 }, // total should use 10*2
                new CartItem { SpeakerId = 2, SpeakerName = "WRONG NAME OK", UnitPrice = 999m, Quantity = 3 }, // total should use 5*3
            };

            // Act
            var result = service.Checkout(userId: 7, cart);

            // Assert (result)
            Assert.True(result.ok);
            Assert.Equal("Checkout complete!", result.message);
            Assert.True(result.orderId > 0);

            // Assert (order created with DB total)
            Assert.Equal(1, ordersRepo.CreateOrderCallCount);
            Assert.Equal(7, ordersRepo.LastCreateOrderUserId);
            Assert.Equal(35m, ordersRepo.LastCreateOrderTotal); // (10*2) + (5*3) = 20 + 15 = 35

            // Assert (items added)
            Assert.Equal(2, ordersRepo.AddOrderItemCallCount);
            Assert.Collection(ordersRepo.AddedItems,
                item =>
                {
                    Assert.Equal(result.orderId, item.OrderId);
                    Assert.Equal(1, item.SpeakerId);
                    Assert.Equal("A", item.SpeakerName);
                    Assert.Equal(10m, item.UnitPrice);
                    Assert.Equal(2, item.Quantity);
                },
                item =>
                {
                    Assert.Equal(result.orderId, item.OrderId);
                    Assert.Equal(2, item.SpeakerId);
                    Assert.Equal("B", item.SpeakerName);
                    Assert.Equal(5m, item.UnitPrice);
                    Assert.Equal(3, item.Quantity);
                });

            // Assert (stock reduced + update called)
            var a = speakersRepo.GetById(1)!;
            var b = speakersRepo.GetById(2)!;

            Assert.Equal(8, a.Stock); // 10 - 2
            Assert.Equal(0, b.Stock); // 3 - 3

            Assert.Equal(2, speakersRepo.UpdateCallCount);
        }

        [Fact]
        public void GetMyOrders_ReturnsOrdersFromRepository()
        {
            // Arrange
            var ordersRepo = new FakeOrderRepository();
            var speakersRepo = new FakeSpeakerRepository();

            var service = new OrderService(ordersRepo, speakersRepo);

            ordersRepo.SeedOrders(123, new List<Order>
            {
                new Order { Id = 1, UserId = 123 },
                new Order { Id = 2, UserId = 123 },
            });

            // Act
            var result = service.GetMyOrders(123);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(2, result[1].Id);
            Assert.Equal(123, ordersRepo.LastGetOrdersByUserId);
        }

        // -------------------------
        // Simple in-memory fakes
        // -------------------------

        private class FakeSpeakerRepository : ISpeakerRepository
        {
            private readonly Dictionary<int, Speaker> _speakers = new();

            public int UpdateCallCount { get; private set; }

            public void Seed(Speaker speaker) => _speakers[speaker.Id] = speaker;

            public List<Speaker> GetAll() => new List<Speaker>(_speakers.Values);

            public Speaker? GetById(int id)
            {
                _speakers.TryGetValue(id, out var sp);
                return sp;
            }

            public void Add(Speaker speaker) => _speakers[speaker.Id] = speaker;

            public void Update(Speaker speaker)
            {
                UpdateCallCount++;
                _speakers[speaker.Id] = speaker;
            }

            public void Delete(int id) => _speakers.Remove(id);
        }

        private class FakeOrderRepository : IOrderRepository
        {
            private int _nextOrderId = 100;

            public int CreateOrderCallCount { get; private set; }
            public int AddOrderItemCallCount { get; private set; }

            public int LastCreateOrderUserId { get; private set; }
            public decimal LastCreateOrderTotal { get; private set; }

            public int LastGetOrdersByUserId { get; private set; }

            public List<OrderItem> AddedItems { get; } = new();

            private readonly Dictionary<int, List<Order>> _ordersByUser = new();

            public void SeedOrders(int userId, List<Order> orders) => _ordersByUser[userId] = orders;

            public int CreateOrder(int userId, decimal total)
            {
                CreateOrderCallCount++;
                LastCreateOrderUserId = userId;
                LastCreateOrderTotal = total;
                return _nextOrderId++;
            }

            public void AddOrderItem(int orderId, OrderItem item)
            {
                AddOrderItemCallCount++;
                AddedItems.Add(item);
            }

            public List<Order> GetOrdersByUser(int userId)
            {
                LastGetOrdersByUserId = userId;
                return _ordersByUser.TryGetValue(userId, out var list) ? list : new List<Order>();
            }
        }
    }
}

