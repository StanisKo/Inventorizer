using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Inventorizer_DataAccess.Data;
using Inventorizer_Models.Models;
using Inventorizer_Models.ViewModels;

using Inventorizer.API.Ebay.Provider;

namespace Inventorizer.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _database;

        private readonly EbayAPIProvider _ebayAPIProvider;

        private int _pageIndex;
        private int _totalPages;

        private const int _PAGE_SIZE = 10;

        private bool _hasPreviousPage
        {
            get => _pageIndex > 1;
        }
        private bool _hasNextPage
        {
            get => _pageIndex < _totalPages;
        }

        public ItemController(ApplicationDbContext db, EbayAPIProvider ebayAPIProvider)
        {
            _database = db;
            _ebayAPIProvider = ebayAPIProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? pageIndex)
        {
            /*
            https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-3.1#add-paging-to-students-index

            Implementing pagination directly on the call to db
            to put legwork on database instead of server memory ...
            */
            _pageIndex = pageIndex ?? 1;

            List<Item> items = await _database.Items
                .AsNoTracking()
                .Include(i => i.Category)
                .Include(i => i.ItemDetail)
                .OrderByDescending(i => i.Price)
                .Skip((_pageIndex - 1) * _PAGE_SIZE)
                .Take(_PAGE_SIZE)
                .ToListAsync();

            _totalPages = (int)Math.Ceiling(items.Count / (double)_PAGE_SIZE);

            IEnumerable<ItemPrices> itemPrices;

            // Retrieve prices for available item names
            try
            {
                itemPrices = await _ebayAPIProvider.RetrieveItemPrices(items.Select(i => i.Name));
            }
            catch (Exception)
            {

            }

            /*
            Stats service here must annotate qs with values

            New view model needed that would also contain Error field (to propagate possible errors to the FE)
            */

            ItemIndexViewModel itemIndexViewModel = new ItemIndexViewModel
            {
                Items = items,
                PageIndex = _pageIndex,
                HasPreviousPage = _hasPreviousPage,
                HasNextPage = _hasNextPage
            };

            return View(itemIndexViewModel);
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