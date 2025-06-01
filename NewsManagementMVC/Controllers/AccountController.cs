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
        public IActionResult Index()
        {
            var listAccounts = _accountService.GetAllAccounts().ToList();
            var viewModels = listAccounts.Select(AccountViewModel.FromSystemAccount).ToList();
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

        //// GET: Account/Delete/5
        //[CustomAuthorize(AccountRole.Admin)]
        //public IActionResult Delete(int? id)
        //{

        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var newsArticle = _accountService.GetAccountById((int)id);
        //    if (newsArticle == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(newsArticle);
        //}


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
            var userId = HttpContext.Session.GetString("UserId");
            var account = _accountService.GetCurrentAccount(int.Parse(userId ?? "0"));
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
            var userId = HttpContext.Session.GetString("UserId");
            var account = _accountService.GetCurrentAccount(int.Parse(userId ?? "0"));
            if (account == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Not Uptdate
            bool isNameChanged = !string.Equals(model.AccountName?.Trim(), account.AccountName, StringComparison.Ordinal);
            bool isEmailChanged = !string.Equals(model.AccountEmail?.Trim(), account.AccountEmail, StringComparison.OrdinalIgnoreCase);

            if (!isNameChanged && !isEmailChanged)
            {
                TempData["InfoMessage"] = "You have already updated your profile. No changes detected.";
                return RedirectToAction("Profile");
            }

            // Update ==> Validate
            if (string.IsNullOrWhiteSpace(model.AccountName) || model.AccountName.Trim().Length <= 2)
            {
                ModelState.AddModelError(nameof(model.AccountName), "Name must be longer than 2 characters.");
            }

            if (string.IsNullOrWhiteSpace(model.AccountEmail) || !model.AccountEmail.Contains("@"))
            {
                ModelState.AddModelError(nameof(model.AccountEmail), "Email must contain '@'.");
            }
            else
            {
                // Check mail existed
                var allEmails = _accountService.GetAllAccountEmails()
                                               .Where(e => !e.Equals(account.AccountEmail, StringComparison.OrdinalIgnoreCase))
                                               .ToList();

                if (allEmails.Contains(model.AccountEmail.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(model.AccountEmail), "Email is already in use.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            account.AccountName = model.AccountName.Trim();
            account.AccountEmail = model.AccountEmail.Trim();

            try
            {
                _accountService.UpdateSystemAccount(account);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating profile: " + ex.Message);
                return View("Profile", model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var account = _accountService.GetCurrentAccount(int.Parse(userId ?? "0"));
            if (account == null)
            {
                TempData["PasswordError"] = "Account not found.";
                return RedirectToAction("Profile");
            }

            if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                TempData["PasswordError"] = "All fields are required.";
                return RedirectToAction("Profile");
            }

            if (!account.AccountPassword.Equals(CurrentPassword))
            {
                TempData["PasswordError"] = "Current password is incorrect.";
                return RedirectToAction("Profile");
            }

            if (NewPassword.Length < 6)
            {
                TempData["PasswordError"] = "New password must be at least 6 characters.";
                return RedirectToAction("Profile");
            }

            if (!NewPassword.Equals(ConfirmPassword))
            {
                TempData["PasswordError"] = "New password and confirmation do not match.";
                return RedirectToAction("Profile");
            }

            try
            {
                account.AccountPassword = NewPassword;
                _accountService.UpdateSystemAccount(account);

                TempData["PasswordSuccess"] = "Password changed successfully.";
            }
            catch (Exception ex)
            {
                TempData["PasswordError"] = "Error changing password: " + ex.Message;
            }

            return RedirectToAction("Profile");
        }

    }
}

