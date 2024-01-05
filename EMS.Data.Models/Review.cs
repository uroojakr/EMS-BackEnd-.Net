using System.ComponentModel.DataAnnotations;

namespace EMS.Data.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Comment { get; set; } = null!;
        [Range(1, 5)]
        public int Rating { get; set; }
        public int UserId { get; set; }
        public int? EventId { get; set; }
        public Events Event { get; set; } = null!;
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; } = null!;



    }
}