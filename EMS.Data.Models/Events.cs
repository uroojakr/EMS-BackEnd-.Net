using System.ComponentModel.DataAnnotations;

namespace EMS.Data.Models
{
    public class Events
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Location { get; set; } = null!; 

        public int? OrganizerId { get; set; }
        public User? Organizer { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = null!;
        public ICollection<VendorEvent> VendorEvents { get; set; } = null!;
        public ICollection<Review> Reviews { get; set; } = null!;

    }
}