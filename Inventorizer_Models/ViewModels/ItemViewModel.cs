using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

using Inventorizer_Models.Models;

namespace Inventorizer_Models.ViewModels
{
    public class ItemViewModel
    {
        public Item Item { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}