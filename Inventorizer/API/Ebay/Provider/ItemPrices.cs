using System.Collections.Generic;

namespace Inventorizer.API.Ebay
{
    public struct ItemNameAndItsPrices
    {
        public string ItemName { get; set; }

        public List<double> ItemPrices { get; set; }
    }

}