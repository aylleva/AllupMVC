using Microsoft.AspNetCore.Identity;

namespace AllupMVC.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname {  get; set; }
        public List<BasketItems> BasketItems { get; set; }
        public List<Order> Orders { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
