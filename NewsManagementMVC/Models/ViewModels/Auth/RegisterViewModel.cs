using System.ComponentModel.DataAnnotations;

namespace NewsManagementMVC.Models.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }
    }

}
