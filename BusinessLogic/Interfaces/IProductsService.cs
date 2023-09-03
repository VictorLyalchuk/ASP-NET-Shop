using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IProductsService
    {
        Task <List<Product>> GetAll();
        //Task <List<Product>> GetAllById(int[] id);
        Task Create(Product product);
        Task Update(Product product);
        Task Delete(int id);
        Task <Product?> Get(int? id);
        
    }
}
