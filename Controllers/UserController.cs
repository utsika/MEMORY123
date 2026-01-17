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
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            UserDatabaseMethods dbm = new UserDatabaseMethods();
            User loggedInUser = dbm.GetUserByUsernameAndPassword(user.UserName, user.Passwords);

            if (loggedInUser == null)
            {
                ModelState.AddModelError("", "Fel användarnamn eller lösenord");
                return View(user);
            }

            HttpContext.Session.SetObject("currentUser", loggedInUser);

            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            UserDatabaseMethods dbm = new UserDatabaseMethods();
            dbm.InsertUser(user);

            HttpContext.Session.SetObject("currentUser", user);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("currentUser");
            return RedirectToAction("Login");
        }

        public IActionResult DeleteAccount()
        {

            var user = HttpContext.Session.GetObject<User>("currentUser");

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            UserDatabaseMethods dbm = new UserDatabaseMethods();
            dbm.DeleteUser(user.UserID);

            HttpContext.Session.Remove("currentUser");

            return RedirectToAction("Login", "User");
        }
    }
}
