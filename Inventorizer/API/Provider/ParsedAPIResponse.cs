using System.Collections.Generic;

namespace Inventorizer.API.Provider
{
    // We only need prices ...
    public struct ParsedAPIResponse
    {
        public List<ItemSummary> ItemSummaries { get; set; }
    }

    public struct ItemSummary
    {
        public Price Price { get; set; }
    }

    public struct Price
    {
        public string Value { get; set; }
    };
}