using deviceInterfacev2.ExtraFiles;
using deviceInterfacev2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace deviceInterfacev2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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

        [HttpGet]
        public IActionResult sendsms()
        {
            SMSOperations sms_sender = new SMSOperations();
            string[] list = { "5072688836" };
            sms_sender.sendSMS1N(list, "deneme");
            return View();
        }
    }
}