// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

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
});