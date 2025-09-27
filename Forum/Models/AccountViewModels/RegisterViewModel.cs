using System.ComponentModel.DataAnnotations;

namespace Forum.Models.AccountViewModels;

public class RegisterViewModel
{
    [Required, MaxLength(32)]
    public string UserName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; }

    [DataType(DataType.Upload)]
    public IFormFile? Avatar { get; set; }
}