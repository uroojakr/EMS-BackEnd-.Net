

using System.ComponentModel.DataAnnotations;

namespace EMS.Business.Models
{
    public class EventsModel
    {

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = null!;

        public int OrganizerId { get; set; }

    }
}
