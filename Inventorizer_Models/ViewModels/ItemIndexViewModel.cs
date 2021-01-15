using System.Collections.Generic;

using Inventorizer_Models.Models;

namespace Inventorizer_Models.ViewModels
{
    public class ItemIndexViewModel
    {
        public IEnumerable<Item> Items { get; set; }

        public int PageIndex { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }
    }
}