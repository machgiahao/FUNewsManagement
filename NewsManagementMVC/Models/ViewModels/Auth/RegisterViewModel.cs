using System.ComponentModel.DataAnnotations;

namespace NewsManagementMVC.Models.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        public string AccountName { get; set; }

        [Required]
        [EmailAddress]
        public string AccountEmail { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string AccountPassword { get; set; }

        [Required]
        [Compare("AccountPassword", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
