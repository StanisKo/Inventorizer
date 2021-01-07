using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Inventorizer_Models.ViewModels;
using Inventorizer.API;

namespace Inventorizer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly EbayAPI _ebayAPI;

        public HomeController(ILogger<HomeController> logger, EbayAPI ebayAPI)
        {
            _logger = logger;
            _ebayAPI = ebayAPI;
        }

        public async Task<IActionResult> Index()
        {
            await _ebayAPI.InitializeAPI();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
