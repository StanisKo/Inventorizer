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

                $(node).siblings(".market-price").first().text(formattedMarketPrice);

                $(node).siblings(".gain-loss").first().text(formattedGainLoss);

            });

            setTimeout(() => $(".alert-info").slideUp(), 1500);
        });
    }
});