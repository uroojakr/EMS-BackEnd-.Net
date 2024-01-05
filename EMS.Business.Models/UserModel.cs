


using EMS.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace EMS.Business.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "User name is required ")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "User name must be between 4 and 10 characters ")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(5, ErrorMessage = "Password must be at least 5  characters")]
        public string Password { get; set; } = null!;

        public UserType UserType { get; set; }
    }
}
