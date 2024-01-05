using System.ComponentModel.DataAnnotations;

namespace EMS.Data.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public Events Event { get; set; } = null!;
    }
}