using System.Diagnostics;
using MEMORY.Models;
using Microsoft.AspNetCore.Mvc;

namespace MEMORY.Controllers
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
            var user = HttpContext.Session.GetObject<User>("currentUser");

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(user);
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

        public IActionResult CreateGame()
        {
            return RedirectToAction("Gamehej", "Game");
        }

        public IActionResult JoinGame()
        {
            return RedirectToAction("JoinGame", "Game");
        }
    }
}
