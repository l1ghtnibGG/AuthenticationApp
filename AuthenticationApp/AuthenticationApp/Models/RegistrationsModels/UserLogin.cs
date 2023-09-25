using System.ComponentModel.DataAnnotations;

namespace AuthenticationApp.Models.RegistrationsModels
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Enter email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
