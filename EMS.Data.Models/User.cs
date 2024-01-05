using System.ComponentModel.DataAnnotations;

namespace EMS.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;

        public UserType UserType { get; set; }
        public ICollection<Events> OrganizedEvents { get; set; } = null!;
        public ICollection<Review> Reviews { get; set; } = null!;
    }
}
