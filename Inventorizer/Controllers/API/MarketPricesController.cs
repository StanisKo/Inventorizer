using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Inventorizer.Shared;
using Inventorizer.API.Ebay.Provider;

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
    exhange of data (item names and prices retrieved from database)
    between ItemController and MarketPricesController via TempData dictionary
    */
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class MarketPricesController : Controller
    {
        private readonly EbayAPIProvider _ebayAPIProvider;

        public MarketPricesController(EbayAPIProvider ebayAPIProvider)
        {
            _ebayAPIProvider = ebayAPIProvider;
        }

        [HttpGet]
        public async Task <ActionResult<IEnumerable<ItemStats>>> GetItemPrices()
        {
            IEnumerable<ItemFromDb> itemsFromDatabase =
                JsonSerializer.Deserialize<IEnumerable<ItemFromDb>>(TempData["itemsFromDatabase"].ToString());

            IEnumerable<ItemPrices> marketPrices = await _ebayAPIProvider.RetrieveItemPrices(
                itemsFromDatabase.Select(item => item.Name)
            );

            List<ItemStats> test = new List<ItemStats>()
            {
                new ItemStats { Name = "test", MarketPrice = 1, ChangeOverTime = 1 }
            };

            return Ok(test);
        }
    }
}