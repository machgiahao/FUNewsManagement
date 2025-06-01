using FUNewsManagementSystem.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DataAccess
{
    public interface IAccountRepository
    {
        SystemAccount GetAccountById(int id);
        SystemAccount GetAccountByEmail(string email);
        void SaveSystemAccount(SystemAccount systemAccount);
        void UpdateSystemAccount(SystemAccount systemAccount);
        void DeleteSystemAccount(int id);
        List<SystemAccount> GetAllAccounts();
        bool IsEmailExisted(string email, int currentAccountId);
    }
}
