using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Inventorizer.Stats;
using Inventorizer.Shared;
using Inventorizer.API.Ebay.Provider;

namespace Inventorizer.Controllers.API
{
    /*
    A Web API controller that requests item prices via EbayAPIProvider,
    does calculations on the degree of depreciation/appreciation via StatsService,
    and returns a serialized collection of structs with the following shape:

    {
        string Name;

        double MarketPrice;

        double GainLoss;
    }

    Inherits from Controller and not ControllerBase to allow exhange of data
    (item names and prices retrieved from database)
    between ItemController and ItemStatsController via TempData dictionary
    */
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ItemStatsController : Controller
    {
        private readonly EbayAPIProvider _ebayAPIProvider;

        private readonly StatsService _statsService;

        public ItemStatsController(EbayAPIProvider ebayAPIProvider, StatsService statsService)
        {
            _ebayAPIProvider = ebayAPIProvider;

            _statsService = statsService;
        }

        [HttpGet]
        public async Task <ActionResult<IEnumerable<ItemStats>>> GetMarketPricesAndStats()
        {
            IEnumerable<ItemFromDb> itemsFromDatabase = JsonSerializer.Deserialize<IEnumerable<ItemFromDb>>(
                TempData["itemsFromDatabase"].ToString()
            );

            IEnumerable<MarketPrices> marketPrices = await _ebayAPIProvider.RetrieveMarketPrices(
                itemsFromDatabase.Select(item => item.Name)
            );

            IEnumerable<StatsInput> statsInputs = itemsFromDatabase.Select(itemFromDb => new StatsInput()
            {
                ItemName = itemFromDb.Name,
                PurchasePrice = itemFromDb.Price,
                MarketPrices = marketPrices.FirstOrDefault(itemFromAPI => itemFromAPI.Name == itemFromDb.Name).Prices
            });

            IEnumerable<ItemStats> itemStats = _statsService.CalculateMarketPriceAndGainLoss(statsInputs);

            return Ok(itemStats);
        }
    }
}