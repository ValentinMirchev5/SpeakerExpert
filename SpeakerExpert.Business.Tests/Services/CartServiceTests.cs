using System.Collections.Generic;
using Xunit;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Services;

namespace SpeakerExpert.Business.Tests.Services
{
    public class CartServiceTests
    {
        [Fact]
        public void GetCart_WhenCurrentIsNull_ReturnsEmptyList()
        {
            // Arrange
            var service = new CartService();
            List<CartItem>? current = null;

            // Act
            var result = service.GetCart(current);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Add_WhenItemNotInCart_AddsNewItemWithQuantity1()
        {
            // Arrange
            var service = new CartService();
            var current = new List<CartItem>();

            // Act
            var cart = service.Add(current, speakerId: 10, name: "JBL Flip 6", price: 59.99m);

            // Assert
            Assert.Single(cart);
            Assert.Equal(10, cart[0].SpeakerId);
            Assert.Equal("JBL Flip 6", cart[0].SpeakerName);
            Assert.Equal(59.99m, cart[0].UnitPrice);
            Assert.Equal(1, cart[0].Quantity);
        }

        [Fact]
        public void Add_WhenItemAlreadyInCart_IncrementsQuantity()
        {
            // Arrange
            var service = new CartService();
            var current = new List<CartItem>
            {
                new CartItem { SpeakerId = 10, SpeakerName = "JBL Flip 6", UnitPrice = 59.99m, Quantity = 1 }
            };

            // Act
            var cart = service.Add(current, speakerId: 10, name: "JBL Flip 6", price: 59.99m);

            // Assert
            Assert.Single(cart);
            Assert.Equal(2, cart[0].Quantity);
        }

        [Fact]
        public void UpdateQuantity_WhenItemNotFound_DoesNothing()
        {
            // Arrange
            var service = new CartService();
            var current = new List<CartItem>
            {
                new CartItem { SpeakerId = 1, SpeakerName = "A", UnitPrice = 10m, Quantity = 1 }
            };

            // Act
            var cart = service.UpdateQuantity(current, speakerId: 999, quantity: 5);

            // Assert
            Assert.Single(cart);
            Assert.Equal(1, cart[0].SpeakerId);
            Assert.Equal(1, cart[0].Quantity);
        }

        [Fact]
        public void UpdateQuantity_WhenQuantityIsPositive_SetsQuantity()
        {
            // Arrange
            var service = new CartService();
            var current = new List<CartItem>
            {
                new CartItem { SpeakerId = 10, SpeakerName = "JBL Flip 6", UnitPrice = 59.99m, Quantity = 1 }
            };

            // Act
            var cart = service.UpdateQuantity(current, speakerId: 10, quantity: 7);

            // Assert
            Assert.Single(cart);
            Assert.Equal(7, cart[0].Quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void UpdateQuantity_WhenQuantityIsZeroOrNegative_RemovesItem(int newQty)
        {
            // Arrange
            var service = new CartService();
            var current = new List<CartItem>
            {
                new CartItem { SpeakerId = 10, SpeakerName = "JBL Flip 6", UnitPrice = 59.99m, Quantity = 3 }
            };

            // Act
            var cart = service.UpdateQuantity(current, speakerId: 10, quantity: newQty);

            // Assert
            Assert.Empty(cart);
        }

        [Fact]
        public void Remove_RemovesAllMatchingSpeakerIds()
        {
            // Arrange
            var service = new CartService();
            var current = new List<CartItem>
            {
                new CartItem { SpeakerId = 1, SpeakerName = "A", UnitPrice = 10m, Quantity = 1 },
                new CartItem { SpeakerId = 2, SpeakerName = "B", UnitPrice = 20m, Quantity = 2 },
            };

            // Act
            var cart = service.Remove(current, speakerId: 1);

            // Assert
            Assert.Single(cart);
            Assert.Equal(2, cart[0].SpeakerId);
        }

        [Fact]
        public void Clear_ReturnsNewEmptyList()
        {
            // Arrange
            var service = new CartService();

            // Act
            var cart = service.Clear();

            // Assert
            Assert.NotNull(cart);
            Assert.Empty(cart);
        }

        [Fact]
        public void Total_WhenCartIsNull_ReturnsZero()
        {
            // Arrange
            var service = new CartService();
            List<CartItem>? current = null;

            // Act
            var total = service.Total(current);

            // Assert
            Assert.Equal(0m, total);
        }

        [Fact]
        public void Total_SumsUnitPriceTimesQuantity()
        {
            // Arrange
            var service = new CartService();
            var current = new List<CartItem>
            {
                new CartItem { SpeakerId = 1, SpeakerName = "A", UnitPrice = 10m, Quantity = 2 },  // 20
                new CartItem { SpeakerId = 2, SpeakerName = "B", UnitPrice = 5m, Quantity = 3 },   // 15
            };

            // Act
            var total = service.Total(current);

            // Assert
            Assert.Equal(35m, total);
        }
    }
}

