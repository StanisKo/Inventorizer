using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Inventorizer.Controllers.Base;

using Inventorizer_Models.Models;
using Inventorizer_Models.ViewModels;
using Inventorizer_DataAccess.Data;

namespace Inventorizer.Controllers
{
    public class CategoryController : CustomBaseController
    {
        private readonly ApplicationDbContext _database;

        public CategoryController(ApplicationDbContext db)
        {
            _database = db;
        }

        [HttpGet]
        public async Task <IActionResult> Index(int? pageIndex)
        {
            int categoriesCount = await _database.Categories.CountAsync();

            _pageIndex = pageIndex ?? 1;

            _totalPages = (int)Math.Ceiling(categoriesCount / (double)_PAGE_SIZE);

            List<Category> categories =  await _database.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((_pageIndex - 1) * _PAGE_SIZE)
                .Take(_PAGE_SIZE)
                .ToListAsync();

            CategoryIndexViewModel categoryIndexViewModel = new CategoryIndexViewModel
            {
                Categories = categories,
                PageIndex = _pageIndex,
                HasNextPage = _hasNextPage,
                HasPreviousPage = _hasPreviousPage
            };

            return View(categoryIndexViewModel);
        }

        [HttpGet]
        public async Task <IActionResult> CreateOrUpdate(int? id)
        {
            if (id == null)
            {
                return View(new Category());
            }

            Category categoryToEdit = await _database.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Category_Id == id);

            if (categoryToEdit == null)
            {
                return NotFound();
            }

            return View(categoryToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> CreateOrUpdate(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Category_Id == 0)
                {
                    _database.Categories.Add(category);
                }
                else
                {
                    _database.Categories.Update(category);
                }

                await _database.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        public async Task <IActionResult> Delete(int id)
        {
            Category categoryToDelete = await _database.Categories.FirstOrDefaultAsync(c => c.Category_Id == id);

            _database.Categories.Remove(categoryToDelete);
            
            await _database.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}