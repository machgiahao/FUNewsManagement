using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using NewsManagementMVC.Models.ViewModels.Auth;

namespace NewsManagementMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;

        public AuthController(IAccountService accountService, INewsArticleService newsArticleService, IConfiguration config)
        {
            _accountService = accountService;
            _config = config;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _accountService.Login(model.AccountEmail, model.AccountPassword);
                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", user.AccountId);
                    HttpContext.Session.SetString("Username", user.AccountName);

                    HttpContext.Session.SetInt32("Role", user.AccountRole ?? -1);
                    if(user.AccountRole == 1)
                    {
                        return RedirectToAction("Index", "Categories");
                    }
                    return RedirectToAction("Index", "NewsArticles"); // Redirect to home NewsArticles
                }
                else
                {
                    ModelState.AddModelError("LOGIN_FAIL", "Invalid username or password");
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session data
            return RedirectToAction("Index", "NewsArticles");
        }
    }
}
