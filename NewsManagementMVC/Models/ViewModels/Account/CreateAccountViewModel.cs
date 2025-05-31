using FUNewsManagementSystem.BusinessObject;
using FUNewsManagementSystem.BusinessObject.Enums;
using System.ComponentModel.DataAnnotations;

namespace NewsManagementMVC.Models.ViewModels.Account
{
    public class CreateAccountViewModel
    {
        public string? AccountName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string AccountEmail { get; set; }

        [Required]
        [EnumDataType(typeof(AccountRole), ErrorMessage = "Invalid account role")]
        public int AccountRole { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string AccountPassword { get; set; }

        public SystemAccount ToSystemAccount()
        {
            return new SystemAccount
            {
                AccountName = this.AccountName,
                AccountEmail = this.AccountEmail,
                AccountPassword = this.AccountPassword,
                AccountRole = this.AccountRole
            };
        }
    }


}
