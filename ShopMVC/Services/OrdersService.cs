using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopMVC.Helper;
using ShopMVC.Interfaces;
using ShopMVC.Models;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace ShopMVC.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly HttpContext _httpContext;
        private readonly ShopMVCDbContext _context;
        private readonly IMapper _mapper;

        public OrdersService(IHttpContextAccessor httpContext, ShopMVCDbContext context, IMapper mapper)
        {
            _httpContext = httpContext.HttpContext;
            _context = context;
            _mapper = mapper;
        }
        public async Task<OrdersProductViewModel> GetAll()
        {
            string userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var orders = await _context.Orders.Where(user => user.UserId == userId).ToListAsync();

            var products = new List<Product>();
            foreach (var order in orders)
            {
                var productIdsJson = order.IdsProduct;
                if (!string.IsNullOrEmpty(productIdsJson))
                {
                    var productIdsDictionary = JsonSerializer.Deserialize<Dictionary<int, int>>(productIdsJson);
                    foreach (var productId in productIdsDictionary.Keys)
                    {
                        var productItem = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
                        if (productItem != null)
                        {
                            products.Add(productItem);
                        }
                    }
                }
            }
            var productsDTO = _mapper.Map<List<ProductDTO>>(products);
            var ordersDTO = _mapper.Map<List<OrderDTO>>(orders);

            var viewModel = new OrdersProductViewModel
            {
                Orders = ordersDTO,
                Products = productsDTO
            };

            return viewModel;
        }
        public async Task CreateAsync()
        {
            Dictionary<int, int> idList = _httpContext.Session.GetObject<Dictionary<int, int>>("mycart");
            if (idList == null) return;
            string _userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            List<int> productIds = idList.Keys.ToList();
            List<Product> products = productIds.Select(id => _context.Products.Find(id)).ToList();

            decimal totalSum = 0;

            foreach (var productId in productIds)
            {
                int quantity = idList[productId];

                var product = products.FirstOrDefault(p => p.Id == productId);

                if (product != null)
                {
                    totalSum += (decimal)(product.Price * quantity);
                }
            }

            Order newOrder = new Order()
            {
                OrderDate = DateTime.Now,
                IdsProduct = JsonSerializer.Serialize(idList),
                TotalPrice = totalSum,
                UserId = _userId
            };
            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();
        }
        public async Task<string> CreateBodyAsync()
        {
            string userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var order = await _context.Orders.Where(user => user.UserId == userId).OrderBy(o => o.Id).LastOrDefaultAsync();
            var productIds = JsonSerializer.Deserialize<Dictionary<int, int>>(order.IdsProduct);

            string body = null;
            foreach (var productId in productIds.Keys)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    double toal_price = (double)(product.Price * productIds[productId]);
                    body += $@"
                    <hr>
                    <img src=""https://localhost:7082/{@product.ImagePath}"" height=""50"" />
                    <img src=""{@product.ImagePath}"" height=""100"" />
                    <br>
                    <span><b>Name:</b> {@product.Name}</span>
                    <br>
                    <span><b>Description:</b> {@product.Description}</span>
                    <br>
                    <span><b>Price:</b> {product.Price} $</span>
                    <br>
                    <a href=""https://localhost:7082/Product/Details/{product.Id}"">View product</a>
                    <br>
                    <span id =""quantity-@productId""><b>Quantity:</b> {@productIds[productId]}</span>
                    <br>
                    <span><b>Total Price:</b> {product.Price * productIds[productId]} $</span>
                    ";
                }
            }

            body += $@"                    
                    <br>
                    <hr>
                    <span><b>Order date:</b> {@order.OrderDate}</span><br>
                    <span><b>Total Sum:</b></span>
                    <span class=""badge bg-primary rounded-pill"">{order.TotalPrice} ₴</span>";
            return body;
        }
        public async Task<string> ViewOrderAsync()
        {
            string userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var order = await _context.Orders.Where(user => user.UserId == userId).OrderBy(o => o.Id).LastOrDefaultAsync();
            return $@"Your Order {order.Id}";
        }
    }
}
