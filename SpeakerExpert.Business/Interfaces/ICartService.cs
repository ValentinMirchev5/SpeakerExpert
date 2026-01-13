using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeakerExpert.Business.Domain;

namespace SpeakerExpert.Business.Interfaces
{
    public interface ICartService
    {
        List<CartItem> GetCart(List<CartItem>? current);
        List<CartItem> Add(List<CartItem>? current, int speakerId, string name, decimal price);
        List<CartItem> UpdateQuantity(List<CartItem>? current, int speakerId, int quantity);
        List<CartItem> Remove(List<CartItem>? current, int speakerId);
        List<CartItem> Clear();
        decimal Total(List<CartItem>? current);
    }
}

