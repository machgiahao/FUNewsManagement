using System.ComponentModel.DataAnnotations;

namespace NewsManagementMVC.Models.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email cannot be blank !")]
        [EmailAddress(ErrorMessage = "Email is not valid !")]
        public string AccountEmail { get; set; }

        [Required(ErrorMessage = "Password cannot be blank !")]
        public string AccountPassword { get; set; }
    }

}
