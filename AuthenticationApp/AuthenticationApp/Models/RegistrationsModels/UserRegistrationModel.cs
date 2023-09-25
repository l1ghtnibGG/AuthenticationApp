using System.ComponentModel.DataAnnotations;

namespace AuthenticationApp.Models.RegistrationsModels;

public class UserRegistrationModel
{
    [Required(ErrorMessage = "Enter email")]
    [EmailAddress]
    public string Email { get; set; }
    
    public string Name { get; set; }

    [Required(ErrorMessage = "Enter password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}