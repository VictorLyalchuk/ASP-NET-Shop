using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IStorageService
    {
        Task<List<Storage>> GetAll();
        Task<Storage?> Get(int? id);
        Storage CreateByQuantity(Product product, int quantity);
        Task Create(Storage storage);
        Task Update(Storage storage);
        Task Delete(int id);
        Task<int> GetStorageQuantityForProduct(int idProduct);
        Task<Product> UpdateQuantity(int productId, int quantity);
        Task UpdateQuantityDecrease(List<int> productId, List<int> quantity);
    }
}
