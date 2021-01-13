using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Inventorizer_DataAccess.Data;
using Inventorizer_Models.Models;
using Inventorizer_Models.ViewModels;

using Inventorizer.API;

namespace Inventorizer.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _database;

        private readonly EbayAPIProvider _ebayAPIProvider;

        public ItemController(ApplicationDbContext db, EbayAPIProvider ebayAPIProvider)
        {
            _database = db;
            _ebayAPIProvider = ebayAPIProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Item> items = await _database.Items
                .AsNoTracking()
                .Include(i => i.Category)
                .Include(i => i.ItemDetail)
                .ToListAsync();

            await _ebayAPIProvider.RetrieveItemPrices(new List<string>() { items.First().Name });

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdate(int? id)
        {
            ItemViewModel itemViewModel = new ItemViewModel
            {
                Categories = await _database.Categories.AsNoTracking().Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Category_Id.ToString()
                }).ToListAsync()
            };

            if (id == null)
            {
                return View(itemViewModel);
            }

            itemViewModel.Item = await _database.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Item_Id == id);

            if (itemViewModel.Item == null)
            {
                return NotFound();
            }

            return View(itemViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(ItemViewModel itemViewModel)
        {
            if (itemViewModel.Item.Item_Id == 0)
            {
                _database.Items.Add(itemViewModel.Item);
            }
            else
            {
                _database.Items.Update(itemViewModel.Item);
            }

            await _database.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdateDetail(int id)
        {
            Item itemToCreateOrUpdateDetail = await _database.Items
                .Include(i => i.ItemDetail)
                .FirstOrDefaultAsync(i => i.Item_Id == id);

            if (itemToCreateOrUpdateDetail == null)
            {
                return NotFound();
            }

            return View(itemToCreateOrUpdateDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdateDetail(Item item)
        {
            if (item.ItemDetail.ItemDetail_Id == 0)
            {
                // Bind item detail with relevant item entity
                ItemDetail itemDetailToCreate = item.ItemDetail;

                itemDetailToCreate.Item_Id = item.Item_Id;

                _database.ItemDetails.Add(itemDetailToCreate);
            }
            else
            {
                _database.ItemDetails.Update(item.ItemDetail);

            }

            await _database.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Item itemToDelete = await _database.Items.FirstOrDefaultAsync(i => i.Item_Id == id);

            _database.Items.Remove(itemToDelete);

            await _database.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}