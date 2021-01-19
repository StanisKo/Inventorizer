namespace Inventorizer.API.ForEx
{
    // We only need US dollars ...
    public struct ParsedExhangeRate
    {
        public Rates Rates { get; set; }
    }

    public struct Rates
    {
        public double USD { get; set; }
    }
}