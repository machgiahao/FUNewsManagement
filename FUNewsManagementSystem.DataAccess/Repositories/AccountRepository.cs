using FUNewsManagementSystem.BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DataAccess
{
    public class AccountRepository : IAccountRepository
    {
        public SystemAccount GetAccountByEmail(string email) {
            using var context = new FunewsManagementContext();
            return context.SystemAccounts.FirstOrDefault(a => a.AccountEmail.ToLower().Equals(email.ToLower()));
        }
        public SystemAccount GetAccountById(int id)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.SystemAccounts.FirstOrDefault(a => a.AccountId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in get account by ID: " + ex.Message);
            }
        }

        public void DeleteSystemAccount(int newsArticleId)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var account = context.SystemAccounts.SingleOrDefault(a => a.AccountId == newsArticleId);
                if (account != null)
                {
                    context.SystemAccounts.Remove(account);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                throw new Exception("Error in deleting account." + ex.Message);
            }
        }

        public List<SystemAccount> GetAllAccounts()
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.SystemAccounts.ToList();
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                throw new Exception("Error in get all accounts." + ex.Message);
            }
        }

        public void SaveSystemAccount(SystemAccount systemAccount)
        {
            try
            {
                using var context = new FunewsManagementContext();
                if (systemAccount.AccountId == 0)
                {
                    systemAccount.AccountId = GetNextAccountId(context);
                }
                context.SystemAccounts.Add(systemAccount);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in adding account: " + ex.Message);
            }
        }

        public void UpdateSystemAccount(SystemAccount systemAccount)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Entry<SystemAccount>(systemAccount).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in updating account: " + ex.Message);
            }
        }

        private short GetNextAccountId(FunewsManagementContext context)
        {
            return (short)((context.SystemAccounts.Any() ? context.SystemAccounts.Max(a => a.AccountId) : 0) + 1);
        }

        public bool IsEmailExisted(string email, int currentAccountId)
        {
            using var context = new FunewsManagementContext();
            return context.SystemAccounts.Any(a => a.AccountEmail.ToLower() == email.ToLower()
                                                  && a.AccountId != currentAccountId);
        }
    }
}
