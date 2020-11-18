using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Inventorizer_DataAccess.Data;
using Inventorizer_Models.Models;
using Inventorizer_Models.ViewModels;

namespace Inventorizer.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _database;

        public ItemController(ApplicationDbContext db)
        {
            _database = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Item> items = _database.Items
                .AsNoTracking()
                .Include(i => i.Category)
                .ToList();

            return View(items);
        }

        [HttpGet]
        public IActionResult CreateOrUpdate(int? id)
        {
            ItemViewModel itemViewModel = new ItemViewModel
            {
                Categories = _database.Categories.Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Category_Id.ToString()
                })
            };

            // Create new item
            if (id == null)
            {
                return View(itemViewModel);
            }

            itemViewModel.Item = _database.Items.FirstOrDefault(i => i.Item_Id == id);

            if (itemViewModel.Item == null)
            {
                return NotFound();
            }

            return View(itemViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrUpdate(ItemViewModel itemViewModel)
        {
            if (itemViewModel.Item.Item_Id == 0)
            {
                _database.Items.Add(itemViewModel.Item);
            }
            else
            {
                _database.Items.Update(itemViewModel.Item);
            }

            _database.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            Item itemToDelete = _database.Items.FirstOrDefault(i => i.Item_Id == id);

            _database.Items.Remove(itemToDelete);
            _database.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}