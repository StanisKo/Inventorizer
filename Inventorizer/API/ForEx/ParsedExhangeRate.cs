namespace Inventorizer.API.ForEx
{
    // We only need EUR rates ...
    public struct ParsedExchangeRate
    {
        public Rates Rates { get; set; }
    }

    public struct Rates
    {
        public double EUR { get; set; }
    }
}