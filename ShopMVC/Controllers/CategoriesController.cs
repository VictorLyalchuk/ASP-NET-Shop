using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.Entities;
using DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;

namespace ShopMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ShopMVCDbContext _context;
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ShopMVCDbContext context, ICategoriesService categoriesService)
        {
            _context = context;
            _categoriesService = categoriesService;
        }
        // GET: Categories
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
              return _context.Categories != null ? 
                          View(await _categoriesService.GetAll()) :
                          Problem("Entity set 'ShopMVCDbContext.Categories'  is null.");
        }
        // GET: Categories/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || await _categoriesService.GetAll() == null)
            {
                return NotFound();
            }

            var category = await _categoriesService.Get((int)id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoriesService.Create(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || await _categoriesService.GetAll() == null)
            {
                return NotFound();
            }

            var category = await _categoriesService.Get((int)id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _categoriesService.Update(category);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || await _categoriesService.GetAll() == null)
            {
                return NotFound();
            }

            var category = await _categoriesService.Get((int)id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _categoriesService.GetAll() == null)
            {
                return Problem("Entity set 'ShopMVCContext.Category'  is null.");
            }
            var category = await _categoriesService.Get((int)id);
            if (category != null)
            {
                await _categoriesService.Delete(category.Id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
