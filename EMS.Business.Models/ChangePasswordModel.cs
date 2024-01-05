

using System.ComponentModel.DataAnnotations;

namespace EMS.Business.Models
{
    public class ChangePasswordModel
    {

        [Required(ErrorMessage = "OldPassword is required")]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "NewPassword is required")]
        [MinLength(5, ErrorMessage = "NewPassword must be at least 5 ")]
        public string NewPassword { get; set; } = null!;
    }
}
