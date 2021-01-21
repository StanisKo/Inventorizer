using System.Collections.Generic;

namespace Inventorizer.Shared
{
    public struct StatsInput
    {
        public string ItemName { get; set; }

        public double PurchasePrice { get; set; }

        public IEnumerable<double> MarketPrices { get; set; }
    }
}