namespace Inventorizer.Controllers.API
{
    public struct ItemPricesStats
    {
        public string Name { get; set; }

        public double MarketPrice { get; set; }

        public float ChangeOverTime { get; set; }
    }
}