using FUNewsManagementSystem.BusinessObject;

namespace NewsManagementMVC.Models.ViewModels.Account
{
    public class AccountViewModel
    {
        public short AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }

        public static AccountViewModel FromSystemAccount(SystemAccount entity)
        {
            return new AccountViewModel
            {
                AccountId = entity.AccountId,
                AccountName = entity.AccountName,
                AccountEmail = entity.AccountEmail,
                AccountRole = entity.AccountRole
            };
        }

        public SystemAccount ToSystemAccount()
        {
            return new SystemAccount
            {
                AccountId = this.AccountId,
                AccountName = this.AccountName,
                AccountEmail = this.AccountEmail,
                AccountRole = this.AccountRole,
            };
        }


    }
}
