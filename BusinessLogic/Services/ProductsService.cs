using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        public ProductsService(IRepository<Product> productRepository, IRepository<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task Create(Product product)
        {
            await _productRepository.Insert(product);
            await _productRepository.Save();
        }
        public async Task Delete(int id)
        {
            var product = await _productRepository.GetByID(id);
            if (product == null) return;
            await Task.Run(
            () =>
            {
                _productRepository.Delete(product);
            });
            await _productRepository.Save();
        }
        public async Task Update(Product product)
        {
            await _productRepository.Update(product);
            await _productRepository.Save();
        }
        public async Task<Product?> Get(int? id)
        {
            return GetAll().Result.FirstOrDefault(p => p.Id == id);
        }
        public async Task<List<Product>> GetAll()
        {
            return _productRepository.Get(includeProperties: new[] { "Category" }).ToList();
        }
        public async Task<List<Category>> GetAllCategory()
        {
            return _categoryRepository.Get().ToList();
        }
    }
}
