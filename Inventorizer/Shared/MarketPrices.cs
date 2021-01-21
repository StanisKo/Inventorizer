using System.Collections.Generic;

namespace Inventorizer.Shared
{
    public struct MarketPrices
    {
        public string Name { get; set; }

        public IEnumerable<double> Prices { get; set; }
    }
}