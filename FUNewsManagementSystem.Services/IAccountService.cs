using FUNewsManagementSystem.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public interface IAccountService
    {
        SystemAccount? Login(string email, string password);
        public void DeleteSystemAccount(int newsArticleId);

        public List<SystemAccount> GetAllAccounts();

        public void CreateSystemAccount(SystemAccount systemAccount);

        public void UpdateSystemAccount(SystemAccount systemAccount);
        SystemAccount GetAccountById(int id);
        public SystemAccount GetCurrentAccount(int id);
        public List<string> GetAllAccountEmails();
        bool IsEmailExisted(string email, int currentAccountId);
        public bool HasAccountChanged(string newName, string newEmail, SystemAccount existing);
        bool ChangePassword(int userId, string currentPassword, string newPassword, out string errorMessage);
    }
}
