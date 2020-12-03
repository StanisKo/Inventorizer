using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Inventorizer_DataAccess.Data;
using Inventorizer_Models.Models;

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
        public IActionResult Index()
        {
            List<Category> categories = _database.Categories.AsNoTracking().ToList();

            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateOrUpdate(int? id)
        {
            if (id == null)
            {
                return View(new Category());
            }

            Category categoryToEdit = _database.Categories
                .AsNoTracking()
                .FirstOrDefault(c => c.Category_Id == id);

            if (categoryToEdit == null)
            {
                return NotFound();
            }

            return View(categoryToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrUpdate(Category category)
        {
            if (ModelState.IsValid)
            {
                // Create
                if (category.Category_Id == 0)
                {
                    _database.Categories.Add(category);
                }
                else
                {
                    _database.Categories.Update(category);
                }

                _database.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        public IActionResult Delete(int id)
        {
            Category categoryToDelete = _database.Categories.FirstOrDefault(c => c.Category_Id == id);

            _database.Categories.Remove(categoryToDelete);
            _database.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}