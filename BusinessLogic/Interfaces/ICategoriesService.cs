using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ICategoriesService
    {
        Task <List<Category>> GetAll();
        Task <Category?> Get(int id);
        Task Create(Category category);
        Task Update(Category category);
        Task Delete(int id);
    }
}
