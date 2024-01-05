using System.ComponentModel.DataAnnotations;

namespace EMS.Data.Models
{
    public class Vendor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public string ContactInformation { get; set; } = null!;

        public ICollection<VendorEvent> VendorEvents { get; set; } = new List<VendorEvent>();
    }
}