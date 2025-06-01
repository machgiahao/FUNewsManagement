using FUNewsManagementSystem.BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsManagementMVC.Models.ViewModels.Account;
using FUNewsManagementSystem.Services;
using FUNewsManagementSystem.BusinessObject.Enums;
using NewsManagementMVC.Attributes;

namespace NewsManagementMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly INewsArticleService _newsArticleService;
        private readonly IConfiguration _config;

        public AccountController(IAccountService accountService, INewsArticleService newsArticleService, IConfiguration config)
        {
            _accountService = accountService;
            _newsArticleService = newsArticleService;
            _config = config;
        }

        // GET: Account/Index
        [CustomAuthorize(AccountRole.Admin)]
        public IActionResult Index(string searchField, string searchString)
        {
            var accounts = _accountService.SearchAccounts(searchField, searchString);
            var viewModels = accounts.Select(AccountViewModel.FromSystemAccount).ToList();
            return View(viewModels);
        }

        // GET: Account/Details/5
        //[HttpGet("Account/Details/{id}")]
        [CustomAuthorize(AccountRole.Admin)]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = _accountService.GetAccountById((int)id);
            var viewModels = AccountViewModel.FromSystemAccount(account);
            if (account == null)
            {
                return NotFound();
            }

            return View(viewModels);
        }

        // GET: Account/Create
        [CustomAuthorize(AccountRole.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(AccountRole.Admin)]
        public IActionResult Create(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = model.ToSystemAccount();
                _accountService.CreateSystemAccount(account);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Account/Edit/5
        [CustomAuthorize(AccountRole.Admin)]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemAccount = _accountService.GetAccountById((int)id);
            var viewModel = AccountViewModel.FromSystemAccount(systemAccount);
            if (systemAccount == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(AccountRole.Admin)]
        public IActionResult Edit(short id, AccountViewModel model)
        {
            if (id != model.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var systemAccount = model.ToSystemAccount();
                    _accountService.UpdateSystemAccount(systemAccount);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemAccountExists(model.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(AccountRole.Admin)]
        public IActionResult DeleteConfirmed(int id)
        {
            var account = _accountService.GetAccountById(id);
            var model = AccountViewModel.FromSystemAccount(account);
            if (model != null)
            {
                _accountService.DeleteSystemAccount(id);
            }
            return RedirectToAction(nameof(Index));
        }
        private bool SystemAccountExists(int id)
        {
            var account = _accountService.GetAccountById(id);
            var model = AccountViewModel.FromSystemAccount(account);
            return (account == null) ? false : true;
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = _accountService.GetCurrentAccount(userId.Value);
            if (account == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var viewModel = AccountViewModel.FromSystemAccount(account);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(AccountViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = _accountService.GetCurrentAccount(userId.Value);
            if (account == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            if (!_accountService.HasAccountChanged(model.AccountName, model.AccountEmail, account))
            {
                TempData["InfoMessage"] = "You have already updated your profile. No changes detected.";
                return RedirectToAction("Profile");
            }

            if (_accountService.IsEmailExisted(model.AccountEmail!.Trim(), account.AccountId))
            {
                ModelState.AddModelError(nameof(model.AccountEmail), "Email is already in use.");
                return View("Profile", model);
            }

            account.AccountName = model.AccountName?.Trim();
            account.AccountEmail = model.AccountEmail?.Trim();

            try
            {
                _accountService.UpdateSystemAccount(account);
                TempData["SuccessMessage"] = "Profile updated successfully.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating profile: " + ex.Message);
                return View("Profile", model);
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["PasswordError"] = string.Join(" ", ModelState.Values
                                                  .SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage));
                return RedirectToAction("Profile");
            }

            var account = HttpContext.Session.GetInt32("UserId");
            if (account == null)
            {
                TempData["PasswordError"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Auth");
            }

            var result = _accountService.ChangePassword(account.Value, model.CurrentPassword, model.NewPassword, out string errorMessage);
            if (!result)
            {
                TempData["PasswordError"] = errorMessage;
                return RedirectToAction("Profile");
            }

            TempData["PasswordSuccess"] = "Password changed successfully.";
            return RedirectToAction("Profile");
        }
    }
}

