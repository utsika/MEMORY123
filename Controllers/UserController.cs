using Microsoft.AspNetCore.Mvc;
using MEMORY.Models;
namespace MEMORY.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            //Get the current user from session
            //User user = HttpContext.Session.GetObject<User>("currentUser");
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            HttpContext.Session.SetObject("currentUser", user);

            //ändra sedan till startsidan när vi har en sådan
            return RedirectToAction("Index", "Home");
        }
    }
}
