using System.Collections.Generic;

using Inventorizer_Models.Models;

namespace Inventorizer_Models.ViewModels
{
    public class ItemIndexViewModel
    {
        public int PageIndex { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public IEnumerable<Item> Items { get; set; }
    }
}