//using Microsoft.AspNetCore.Mvc;
//using MEMORY.Models;
//namespace MEMORY.Controllers
//{
//    public class TestController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }

//        public IActionResult InsertUser()
//        {
//            UserDetail userDetail = new UserDetail();
//            UserMethod userMethod = new UserMethod();
//            int i = 0;
//            string errorMessage = "";

//            userDetail.UserName = "TestUser";
//            userDetail.Email = "...@test.com";
//            userDetail.Passwords = "TestPassword";

//            i = userMethod.InsertUser(userDetail, out errorMessage);
//            ViewBag.errorMessage = errorMessage;
//            ViewBag.antal = 1;

//            return View();
//        }
//    }
//}
