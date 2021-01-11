namespace Inventorizer.API.Auth
{
    public struct ParsedAuth
    {
        public string access_token { get; set; }

        public int expires_in { get; set; }
    }
}