using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;

namespace SpeakerExpert.Business.Services
{
    public class CartService : ICartService
    {
        public List<CartItem> GetCart(List<CartItem>? current)
        {
            return current ?? new List<CartItem>();
        }

        public List<CartItem> Add(List<CartItem>? current, int speakerId, string name, decimal price)
        {
            var cart = GetCart(current);

            var item = cart.FirstOrDefault(x => x.SpeakerId == speakerId);
            if (item == null)
            {
                cart.Add(new CartItem
                {
                    SpeakerId = speakerId,
                    SpeakerName = name,
                    UnitPrice = price,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity += 1;
            }

            return cart;
        }

        public List<CartItem> UpdateQuantity(List<CartItem>? current, int speakerId, int quantity)
        {
            var cart = GetCart(current);

            var item = cart.FirstOrDefault(x => x.SpeakerId == speakerId);
            if (item == null) return cart;

            if (quantity <= 0)
                cart.Remove(item);
            else
                item.Quantity = quantity;

            return cart;
        }

        public List<CartItem> Remove(List<CartItem>? current, int speakerId)
        {
            var cart = GetCart(current);
            cart.RemoveAll(x => x.SpeakerId == speakerId);
            return cart;
        }

        public List<CartItem> Clear() => new List<CartItem>();

        public decimal Total(List<CartItem>? current)
        {
            var cart = GetCart(current);
            return cart.Sum(x => x.UnitPrice * x.Quantity);
        }
    }
}
