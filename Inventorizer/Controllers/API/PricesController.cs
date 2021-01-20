using System;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Inventorizer.API.Ebay.Provider;
using Inventorizer.Shared;

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

    Inheriting from Controller and not ControllerBase to allow
    exhange of data (item names and initial prices)
    between ItemController and MarketPricesController via TempData dictionary
    */
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class PricesController : Controller
    {
        private readonly EbayAPIProvider _ebayAPIProvider;

        public PricesController(EbayAPIProvider ebayAPIProvider)
        {
            _ebayAPIProvider = ebayAPIProvider;
        }

        [HttpGet]
        public async Task <ActionResult<IEnumerable<ItemPricesStats>>> GetItemPrices()
        {
            IEnumerable<ItemNameAndPrice> itemNamesAndPrices =
                JsonSerializer.Deserialize<IEnumerable<ItemNameAndPrice>>(TempData["itemNamesAndPrices"].ToString());

            IEnumerable<ItemPrices> itemPrices = await _ebayAPIProvider.RetrieveItemPrices(
                itemNamesAndPrices.Select(item => item.Name)
            );

            List<ItemPricesStats> test = new List<ItemPricesStats>()
            {
                new ItemPricesStats { Name = "test", MarketPrice = 1, ChangeOverTime = 1 }
            };

            return Ok(test);
        }
    }
}