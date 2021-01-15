using System.Net.Mime;

using Microsoft.AspNetCore.Mvc;

/*
1. Map prices to endpoint instead of requesting in MVC controller

https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-5.0

https://stackoverflow.com/questions/44914722/how-to-add-web-api-controller-to-an-existing-asp-net-core-mvc
*/

namespace Inventorizer.Controllers.API
{
    /*
    A Web API controller that requests item prices via EbayAPIProvider,
    does calculations on whether items depreciate/appreciate over time (and change rate in %) via Stats,
    and returns a serialized collection of structs with the following shape:

    {
        string ItemName;

        double AverageItemPrice;

        float ChangeOverTime;
    }

    Expects a collection of item names to fetch prices for, that is supplied via AJAX request from FE
    */
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class PricesAPIController : ControllerBase
    {
        
    }
}