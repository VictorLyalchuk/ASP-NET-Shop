using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class StorageService : IStorageService
    {
        private readonly ShopMVCDbContext _context;
        public StorageService(ShopMVCDbContext context)
        {
            _context = context;
        }
        public async Task Create(Storage storage)
        {
            await _context.Storage.AddAsync(storage);
            await _context.SaveChangesAsync();
        }
        public Storage CreateByQuantity(Product product, int quantity)
        {
            Storage storage = new Storage
            {
                ProductId = product.Id,
                Products = product,
                ProductQuantity = quantity
            };
            return storage;
        }
        public async Task<Storage?> Get(int? id)
        {
            return await _context.Storage.FirstOrDefaultAsync(p => p.ProductId == id);

        }
        public async Task <Product> UpdateQuantity(int productId, int quantity)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == productId);
            if (product != null)
            {
                var existingStorage = await Get(productId);
                if (existingStorage == null)
                {
                    Storage storage = CreateByQuantity(product, quantity);
                    await Create(storage);
                    product.StorageId = storage.Id;
                    product.Storage = storage;
                }
                else
                {
                    existingStorage.ProductQuantity += quantity;
                    await Update(existingStorage);
                    product.StorageId = existingStorage.Id;
                }

                var newpoduct = _context.Products.Update(product);
            }
            return product;
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Storage>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task Update(Storage storage)
        {
            _context.Update(storage);
            await _context.SaveChangesAsync();
        }
        public async Task <int> GetStorageQuantityForProduct (int idProduct)
        {
            var storage = await _context.Storage.FirstOrDefaultAsync(p => p.ProductId == idProduct);
            return storage != null ? storage.ProductQuantity : 0;
            //return await _context.Storage.Where(p => p.ProductId == idProduct).FirstOrDefault(p => p.ProductQuantity);
        }
    }
}
