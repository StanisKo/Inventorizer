namespace Inventorizer.API
{
    public struct ParsedAuth
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string errorString { get; set; }
    }
}