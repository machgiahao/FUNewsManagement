using FUNewsManagementSystem.BusinessObject;
using FUNewsManagementSystem.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _iAccountRepository;
        private readonly IConfiguration _config;

        public AccountService(IAccountRepository iAccountRepository, IConfiguration config)
        {
            this._iAccountRepository = iAccountRepository;
            this._config = config;
        }

        public SystemAccount? Login(string email, string password)
        {
            // Check default admin from appsettings
            var adminEmail = _config["AdminAccount:Email"];
            var adminPassword = _config["AdminAccount:Password"];

            if (email == adminEmail && password == adminPassword)
            {
                return new SystemAccount
                {
                    AccountId = 0,
                    AccountName = "Admin",
                    AccountEmail = email,
                    AccountPassword = password,
                    AccountRole = 0
                };
            }

            var user = _iAccountRepository.GetAccountByEmail(email);
            if (user != null && user.AccountPassword == password)
            {
                return user;
            }

            return null;
        }


        public List<SystemAccount> GetAllAccounts() => _iAccountRepository.GetAllAccounts();
        public void DeleteSystemAccount(int newsArticleId) => _iAccountRepository.DeleteSystemAccount(newsArticleId);

        public void CreateSystemAccount(SystemAccount systemAccount) => _iAccountRepository.SaveSystemAccount(systemAccount);

        public void UpdateSystemAccount(SystemAccount systemAccount) => _iAccountRepository.UpdateSystemAccount(systemAccount);
        public SystemAccount GetAccountById(int id) => _iAccountRepository.GetAccountById(id);

        public List<SystemAccount> SearchAccounts(string searchField, string searchString)
        {
            var accounts = GetAllAccounts();
            if (string.IsNullOrEmpty(searchString))
                return accounts;

            switch (searchField)
            {
                case "AccountEmail":
                    return accounts.Where(a => a.AccountEmail != null && a.AccountEmail.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                case "AccountRole":
                    return accounts.Where(a => a.AccountRole.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                default:
                    return accounts.Where(a => a.AccountName != null && a.AccountName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        public void Register(string name, string email, string password)
        {
            if (_iAccountRepository.GetAccountByEmail(email) != null)
                throw new Exception("Email already exist");

            var account = new SystemAccount
            {
                //AccountId = Uuid
                AccountName = name,
                AccountEmail = email,
                AccountPassword = password,
                AccountRole = 2
            };
            _iAccountRepository.SaveSystemAccount(account);
        }

        public SystemAccount GetCurrentAccount(int id) => _iAccountRepository.GetAccountById(id);

        public List<string> GetAllAccountEmails() => _iAccountRepository.GetAllAccounts()
                                                                       .Where(a => !string.IsNullOrWhiteSpace(a.AccountEmail))
                                                                       .Select(a => a.AccountEmail.ToLower())
                                                                       .ToList();

        public bool IsEmailExisted(string email, int currentAccountId)
        {
            return _iAccountRepository.IsEmailExisted(email, currentAccountId);
        }

        public bool HasAccountChanged(string newName, string newEmail, SystemAccount existing)
        {
            return !string.Equals(newName?.Trim(), existing.AccountName, StringComparison.Ordinal) ||
                   !string.Equals(newEmail?.Trim(), existing.AccountEmail, StringComparison.OrdinalIgnoreCase);
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword, out string errorMessage)
        {
            var account = _iAccountRepository.GetAccountById(userId);
            if (account == null)
            {
                errorMessage = "Account not found.";
                return false;
            }

            if (!account.AccountPassword.Equals(currentPassword))
            {
                errorMessage = "Current password is incorrect.";
                return false;
            }

            account.AccountPassword = newPassword;

            try
            {
                _iAccountRepository.UpdateSystemAccount(account);
                errorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Error changing password: " + ex.Message;
                return false;
            }
        }

    }

}
