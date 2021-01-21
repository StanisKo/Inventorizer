using System.Linq;
using System.Collections.Generic;

using Inventorizer.Shared;
using Inventorizer.API.ForEx;

namespace Inventorizer.Stats
{
    public class StatsModule
    {
        private readonly ForExAPIService _forExAPIService;

        public StatsModule(ForExAPIService forExAPIService)
        {
            _forExAPIService = forExAPIService;
        }

        public IEnumerable<ItemStats> CalculateGainLoss(IEnumerable<StatsInput> statsInput)
        {
            IEnumerable<ItemStats> itemStats = statsInput.Select(input => {
                /*
                If market prices are available average them out and translate from USD to EUR
                */
                double marketPrice = input.MarketPrices.Count() > 0
                    ? input.MarketPrices.Sum() / input.MarketPrices.Count() * _forExAPIService.ParsedExchangeRate.Rates.EUR
                    : 0;

                /*
                If market prices are available, calculate gain/loss
                where gain/loss is difference between purchase price and market price
                divided by purchase price and multiplied by 100 for per cent
                */
                double gainLoss = marketPrice > 0
                    ? (marketPrice - input.PurchasePrice) / input.PurchasePrice * 100
                    : 0;

                return new ItemStats()
                {
                    Name = input.ItemName,
                    MarketPrice = marketPrice,
                    GainLoss = gainLoss
                };
            });

            return itemStats;
        }
    }
}