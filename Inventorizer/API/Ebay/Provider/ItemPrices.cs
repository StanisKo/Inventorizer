using System.Collections.Generic;

namespace Inventorizer.API.Ebay.Provider
{
    public struct ItemPrices
    {
        public string Name { get; set; }

        public IEnumerable<double> Prices { get; set; }
    }
}