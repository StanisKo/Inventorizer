$(() => {
    $("#datepicker").datepicker({});

    $(".price").text((_, text) => {
        const value = Number(text);

        return `€ ${value.toFixed(2)}`;
    });

    $(".date-of-purchase").text((_, text) => {
        const dateOfPurchase = new Date(text);

        return `${dateOfPurchase.getDate()}-${dateOfPurchase.getMonth() + 1}-${dateOfPurchase.getFullYear()}`
    });

    // Request item prices from item index view
    if (window.location.pathname === '/Item') {
        $(".alert-info").slideDown();

        const url = new URL(`${window.location.protocol}//${window.location.host}/api/marketprices`);

        $.get(url).done((itemStats) => {

            $(".item-name").each((_, node) => {
                const relevantItem = itemStats.find(item => item.name === $(node).text().trim());

                const formattedMarketPrice = Number(relevantItem.marketPrice).toFixed(2);

                const formattedGainLoss = Number(relevantItem.gainLoss).toFixed(2);

                const marketPriceNode = $(node).siblings(".market-price").first();

                const gainLossNode = $(node).siblings(".gain-loss").first();

                marketPriceNode.text(`€ ${formattedMarketPrice}`);

                gainLossNode.text(`€ ${formattedGainLoss}`);

                if (formattedGainLoss > 0)
                {
                    gainLossNode.css({ "color": "green"});
                }
                else if (formattedGainLoss < 0)
                {
                    gainLossNode.css({ "color": "red"});
                }

            });

            setTimeout(() => $(".alert-info").slideUp(), 1500);
        });
    }
});