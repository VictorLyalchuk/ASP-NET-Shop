using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ProductsService : IProductsService
    {
        private readonly ShopMVCDbContext _context;
        public ProductsService(ShopMVCDbContext context)
        {
            _context = context;
        }
        public async Task Create(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return;
            await Task.Run(
            () =>
            {
                _context.Remove(product);
            });
            await _context.SaveChangesAsync();
        }
        public async Task Update(Product product)
        {
            await Task.Run
              (
              () =>
              {
                  _context.Attach(product);
                  _context.Entry(product).State = EntityState.Modified;
              });
            await _context.SaveChangesAsync();
        }
        public async Task <Product?> Get(int? id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

        }
        public async Task <List<Product>> GetAll()
        {
           return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        //public async Task<List<Product>> GetAllById(int[] id)
        //{
        //    List<Product> products = id.Select(a => Get(a)).ToList();
        //    return products;
        //}
    }
}
