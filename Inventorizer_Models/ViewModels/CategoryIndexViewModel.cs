using System.Collections.Generic;

using Inventorizer_Models.Models;

namespace Inventorizer_Models.ViewModels
{
    public class CategoryIndexViewModel
    {
        public int PageIndex { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public IEnumerable<Category> Categories { get; set; }
    }
}