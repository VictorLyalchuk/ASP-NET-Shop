using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopMVC.Helper;
using ShopMVC.Models;
using System.Diagnostics;

namespace ShopMVC.Controllers
{
    //[Controller]
    //public class Home
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopMVCDbContext _context;
        private readonly ICategoriesService _categoriesService;
        private readonly IProductsService _productsService;

        public HomeController(ILogger<HomeController> logger, ShopMVCDbContext context, ICategoriesService categoriesService, IProductsService productsService)
        {
            _logger = logger;
            _context = context;
            _categoriesService = categoriesService;
            _productsService = productsService;
        }
        public async Task<IActionResult> Index(int? category_id)
        {
            //Example using Cookies
            //append Cookie
            //HttpContext.Response.Cookies.Append("name", "Tetiana");
            //get Cookie
            //ViewBag.NameAuthor = HttpContext.Request.Cookies["name"];
            //delete Cookie
            //HttpContext.Response.Cookies.Delete("name");

            List<Category> categories = await _categoriesService.GetAll();
            categories.Insert(0, new Category { Id = 0, Name = "All", Description = "All Products" });
            ViewBag.ListCategories = categories;
            ViewData["ListCategories"] = categories;
            //var products = _context.Products.Include(product => product.Category).ToList();
            //var products = _productsService.GetAllByPrice().Result;
            var products = _productsService.GetAll().Result;
            if (category_id!=null && category_id > 0)
            {
                products = products.Where(p => p.CategoryId == category_id).ToList();
            }

            var allProducts = products.Select(
                p=>new ProductCartViewModel { 
                    Product=p,
                    IsInCart=IsProductInCart(p.Id),
                    StorageQuantity = GetStorageQuantityForProduct(p.Id)
                }
                ).ToList();

            //for defination active link or disabled
            if (category_id == null)
            {
                ViewBag.ActiveCategoryId = 0;
            }
            else {
                ViewBag.ActiveCategoryId = category_id;
            }

            var topProducts = await GetTop4ProductsAsync();
            return View(new HomePageViewModel
            {
                AllProducts = allProducts,
                TopProducts = topProducts
            });
            //return View(productsCartViewModel);
        }
        private bool IsProductInCart(int id) {
            Dictionary<int, int> IdCount = HttpContext.Session.GetObject<Dictionary<int, int>>("mycart");
            if (IdCount == null) return false;
            return IdCount.ContainsKey(id);
        }
        private int GetStorageQuantityForProduct(int productId)
        {
            var storageInfo = _context.Storage.FirstOrDefault(s => s.ProductId == productId);
            return storageInfo?.ProductQuantity ?? 0; 
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult About()
        {
            var service = HttpContext.RequestServices.GetServices<UserManager<User>>();
            return View(service);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private async Task<List<ProductCartViewModel>> GetTop4ProductsAsync()
        {
            var orders = await _context.Orders.ToListAsync();
            var productSalesCount = new Dictionary<int, int>();

            foreach (var order in orders)
            {
                var idList = JsonConvert.DeserializeObject<Dictionary<int, int>>(order.IdsProduct);
                foreach (var kvp in idList)
                {
                    var productId = kvp.Key;
                    var quantity = kvp.Value;

                    if (productSalesCount.ContainsKey(productId))
                    {
                        productSalesCount[productId] += quantity;
                    }
                    else
                    {
                        productSalesCount[productId] = quantity;
                    }
                }
            }
            var top4Products = productSalesCount.OrderByDescending(kvp => kvp.Value)
                                               .Take(4)
                                               .ToList();

            var top4ProductData = new List<ProductCartViewModel>();
            foreach (var kvp in top4Products)
            {
                var productId = kvp.Key;
                //var product = await _context.Products.FindAsync(productId);
                var product = await _productsService.Get(productId);
                if (product != null)
                {
                    var productViewModel = new ProductCartViewModel
                    {
                        Product = product,
                        IsInCart = false, 
                        Quantity = 0,   
                        StorageQuantity = GetStorageQuantityForProduct(productId)
                    };

                    top4ProductData.Add(productViewModel);
                }
            }
            return top4ProductData;

        }
    }
}