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

        $.get(url).done((MarketPrices) => {
            console.log(MarketPrices);

            setTimeout(() => $(".alert-info").slideUp(), 1500);
        });
    }
});