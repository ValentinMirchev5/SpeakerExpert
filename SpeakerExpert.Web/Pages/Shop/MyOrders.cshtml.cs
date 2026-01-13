using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using SpeakerExpert.Business.Domain;
using SpeakerExpert.Business.Interfaces;
using System.Security.Claims;

namespace SpeakerExpert.Web.Pages.Shop
{
    [Authorize]
    public class MyOrdersModel : PageModel
    {
        private readonly IOrderService _orders;

        public MyOrdersModel(IOrderService orders)
        {
            _orders = orders;
        }

        public List<Order> Orders { get; set; } = new();

        public void OnGet()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Orders = _orders.GetMyOrders(userId);
        }
    }
}

