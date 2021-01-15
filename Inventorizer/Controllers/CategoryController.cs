using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Inventorizer_Models.Models;
using Inventorizer_DataAccess.Data;

/*
TODO:

Add pagination
*/

namespace Inventorizer.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _database;

        public CategoryController(ApplicationDbContext db)
        {
            _database = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Category> categories =  await _database.Categories.AsNoTracking().ToListAsync();

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdate(int? id)
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
        public async Task<IActionResult> CreateOrUpdate(Category category)
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

        public async Task<IActionResult> Delete(int id)
        {
            Category categoryToDelete = await _database.Categories.FirstOrDefaultAsync(c => c.Category_Id == id);

            _database.Categories.Remove(categoryToDelete);
            
            await _database.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}