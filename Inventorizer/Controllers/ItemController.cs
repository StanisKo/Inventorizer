using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Inventorizer_DataAccess.Data;
using Inventorizer_Models.Models;
using Inventorizer_Models.ViewModels;

using Inventorizer.Controllers.Base;
using Inventorizer.Shared;

namespace Inventorizer.Controllers
{
    public class ItemController : CustomBaseController
    {
        private readonly ApplicationDbContext _database;

        public ItemController(ApplicationDbContext db)
        {
            _database = db;
        }

        [HttpGet]
        public async Task <IActionResult> Index(int? pageIndex)
        {
            int itemsCount = await _database.Items.CountAsync();

            _pageIndex = pageIndex ?? 1;

            _totalPages = (int)Math.Ceiling(itemsCount / (double)_PAGE_SIZE);

            List<Item> items = await _database.Items
                .AsNoTracking()
                .Include(i => i.Category)
                .Include(i => i.ItemDetail)
                .OrderByDescending(i => i.Price)
                .Skip((_pageIndex - 1) * _PAGE_SIZE)
                .Take(_PAGE_SIZE)
                .ToListAsync();

            /*
            Store names and prices in TempData dict
            so that ItemStatsController can access them and perform requests to API and calculations

            We also serialize it since TempData does not support storing complex types
            */
            IEnumerable<ItemFromDb> itemsFromDatabase = items.Select(
                item => new ItemFromDb { Name = item.Name, Price = item.Price }
            );

            TempData["itemsFromDatabase"] = JsonSerializer.Serialize<IEnumerable<ItemFromDb>>(itemsFromDatabase);

            ItemIndexViewModel itemIndexViewModel = new ItemIndexViewModel
            {
                Items = items,
                PageIndex = _pageIndex,
                HasNextPage = _hasNextPage,
                HasPreviousPage = _hasPreviousPage
            };

            return View(itemIndexViewModel);
        }

        [HttpGet]
        public async Task <IActionResult> CreateOrUpdate(int? id)
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
        public async Task <IActionResult> CreateOrUpdate(ItemViewModel itemViewModel)
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
        public async Task <IActionResult> CreateOrUpdateDetail(int id)
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
        public async Task <IActionResult> CreateOrUpdateDetail(Item item)
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
                Item itemToUpdateDetail = await _database.Items
                    .Include(i => i.ItemDetail)
                    .FirstOrDefaultAsync(i => i.Item_Id == item.Item_Id);

                itemToUpdateDetail.ItemDetail = item.ItemDetail;

                _database.Items.Attach(itemToUpdateDetail);
            }

            await _database.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task <IActionResult> Delete(int id)
        {
            Item itemToDelete = await _database.Items.FirstOrDefaultAsync(i => i.Item_Id == id);

            _database.Items.Remove(itemToDelete);

            await _database.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}