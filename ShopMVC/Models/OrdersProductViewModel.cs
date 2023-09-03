using DataAccess.Entities;

namespace ShopMVC.Models
{
    public class OrdersProductViewModel
    {
        public List<Order> Orders { get; set; }
        public List<Product> Products { get; set; }
    }
}
