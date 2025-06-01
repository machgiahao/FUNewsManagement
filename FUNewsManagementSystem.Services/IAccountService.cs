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
        List<SystemAccount> SearchAccounts(string searchField, string searchString);
        void Register(string name, string email, string password);


    }
}
