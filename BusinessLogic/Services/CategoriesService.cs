using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly IRepository<Category> _categoryRepository;
        public CategoriesService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task Create(Category category)
        {
            await _categoryRepository.Insert(category);
            await _categoryRepository.Save();
        }

        public async Task Delete(int id)
        {
            var category = await _categoryRepository.GetByID(id);
            if (category == null) return;

            await Task.Run(
            () =>
            {
                _categoryRepository.Delete(category);
            });
            await _categoryRepository.Save();
        }
        public async Task Update(Category category)
        {
            await _categoryRepository.Update(category);
            await _categoryRepository.Save();
        }

        public async Task<Category?> Get(int id)
        {
            return GetAll().Result.FirstOrDefault(p => p.Id == id);
        }

        public async Task<List<Category>> GetAll()
        {
            return _categoryRepository.Get().ToList();
        }

    }
}
