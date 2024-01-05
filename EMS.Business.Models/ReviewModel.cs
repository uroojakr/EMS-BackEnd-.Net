using System.ComponentModel.DataAnnotations;

namespace EMS.Business.Models
{
    public class ReviewModel
    {

        [Required(ErrorMessage = "Comment is required")]
        public string Comment { get; set; } = null!;

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        public int UserId { get; set; }
        public int? EventId { get; set; }
        public int? VendorId { get; set; }
    }
}