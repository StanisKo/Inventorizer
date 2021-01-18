using System;
using System.Net.Mime;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Inventorizer.API.Ebay.Provider;

/*
1. Map prices to endpoint instead of requesting in MVC controller

https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-5.0

https://stackoverflow.com/questions/44914722/how-to-add-web-api-controller-to-an-existing-asp-net-core-mvc

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
            // Null checks must be improved
            if (itemNames == null || itemNames.Length == 0)
            {
                return BadRequest("itemNames: <string[]> is missing from the request");
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