using System.Collections.Generic;

namespace Inventorizer.API.Ebay.Provider
{
    public struct ItemNameAndItsPrices
    {
        public string ItemName { get; set; }

        public IEnumerable<double> ItemPrices { get; set; }
    }
}