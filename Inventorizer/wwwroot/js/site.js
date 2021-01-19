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

        const key = "itemNames";

        // Grab names from html nodes
        const itemNames = $(".itemName").map((_, node) => $(node).text().trim());

        // Declare base URL
        const baseURL = new URL(`${window.location.protocol}//${window.location.host}/api/prices`);

        // Add names to querystring
        const querystring = itemNames.toArray().map((name) => `${key}=${name}`).join("&");

        $.get(`${baseURL}?${querystring}`).done((itemPrices) => {
            console.log(itemPrices);

            $(".alert-info").slideUp();
        });
    }
});