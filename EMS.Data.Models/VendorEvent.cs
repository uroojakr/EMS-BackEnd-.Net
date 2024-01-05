using System.ComponentModel.DataAnnotations;

namespace EMS.Data.Models
{
    public class VendorEvent
    {
        [Key]
        public int Id { get; set; }
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; } = null!;
        public int EventId { get; set; }
        public Events Event { get; set; } = null!;
    }
}