using System.ComponentModel.DataAnnotations;

namespace EMS.Business.Models
{
    public class VendorModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Contact information is required")]
        [EmailAddress(ErrorMessage = "Invalid email adress format")]
        public string ContactInformation { get; set; } = null!;

    }
}
