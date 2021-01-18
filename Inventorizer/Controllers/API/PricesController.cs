using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Inventorizer.API.Ebay.Provider;

/*
1. Map prices to endpoint instead of requesting in MVC controller

https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-5.0

https://www.youtube.com/watch?v=fmvcAzHpsk8 33:25
*/

namespace Inventorizer.Controllers.API
{
    /*
    A Web API controller that requests item prices via EbayAPIProvider,
    does calculations on whether items depreciate/appreciate over time (and change rate in %) via Stats,
    and returns a serialized collection of structs with the following shape:

    {
        string Name;

        double MarketPrice;

        float ChangeOverTime;
    }

    Expects a collection of item names to fetch prices for, that is supplied via AJAX request from FE
    */
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class PricesController : ControllerBase
    {
        private readonly EbayAPIProvider _ebayAPIProvider;

        public PricesController(EbayAPIProvider ebayAPIProvider)
        {
            _ebayAPIProvider = ebayAPIProvider;
        }

        [HttpGet]
        public async Task <ActionResult<IEnumerable<ItemPricesStats>>> GetItemPrices([FromQuery] string[] itemNames)
        {

            // Check if param is in query string
            if (!Request.Query.ContainsKey("itemNames"))
            {
                return BadRequest("itemNames: <string[]> is missing from the request");
            }

            // Check if param has values
            if (Array.Exists(itemNames, item => String.IsNullOrEmpty(item)))
            {
                return BadRequest("itemNames: <string[]> is empty or at least one value is null or empty");
            }

            // Check if param contains strings only
            if (itemNames.Any(name => int.TryParse(name, out _)))
            {
                return BadRequest("itemNames: <string[]> should not contain integers");
            }

            IEnumerable<ItemPrices> itemPrices = await _ebayAPIProvider.RetrieveItemPrices(itemNames);

            List<ItemPricesStats> test = new List<ItemPricesStats>()
            {
                new ItemPricesStats { Name = "test", MarketPrice = 1, ChangeOverTime = 1 }
            };

            return Ok(test);
        }
    }
}