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

        public IActionResult Index()
        {
            List<Category> categories = _database.Categories.AsNoTracking().ToList();

            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateOrUpdate(int? requestedId)
        {
            // Return view with category to create
            if (requestedId == null)
            {
                return View(new Category());
            }

            // Return view with category to update
            Category categoryToEdit = _database.Categories
                .AsNoTracking()
                .FirstOrDefault(c => c.Category_Id == requestedId);

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
                    _database.Add(category);
                }
                else
                {
                    _database.Update(category);
                }

                _database.SaveChanges();

                // Redirect back to index
                return RedirectToAction(nameof(Index));
            }

            // If the model is not valid, we return the passed category object back with the view
            return View(category);
        }

        public IActionResult Delete(int requestedId)
        {
            Category categoryToDelete = _database.Categories.FirstOrDefault(c => c.Category_Id == requestedId);

            _database.Categories.Remove(categoryToDelete);
            _database.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}