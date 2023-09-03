using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopMVC.Models;

namespace ShopMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductsService _productsService;
        private readonly ICategoriesService _categoriesService;
        public ProductController(IProductsService productsService, ICategoriesService categoriesService)
        {
            _productsService = productsService;
            _categoriesService = categoriesService;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            List<Category> categories= await _categoriesService.GetAll();
            ViewBag.ListCategories = categories;
            ViewData["ListCategories"] = categories;
            var products = await _productsService.GetAll();
            return View(products);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id, string returnUrl = null) 
        {
            var product = await _productsService.Get(id);
            if (product == null) return NotFound();
            ViewBag.ReturnUrl = returnUrl;

            var productCartViewModel = new ProductCartViewModel
            {
                Product = product,
            };
            return View(productCartViewModel);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            await _productsService.Delete((int)id);
            return RedirectToAction(nameof(Index), "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Create() {
            var categories = await _categoriesService.GetAll();
            ViewBag.ListCategory = new SelectList(categories,"Id","Name");
           return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoriesService.GetAll();
                ViewBag.ListCategory = new SelectList(categories, "Id", "Name");
                return View(product);
            }
            await _productsService.Create(product);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id) {

            var product = await _productsService.Get(id);
            if (product != null)
            {
                var categories = await _categoriesService.GetAll();
                ViewBag.ListCategory = new SelectList(categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            await _productsService.Update(product);
            return RedirectToAction("Index");
        }
    }
}
