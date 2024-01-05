
using EMS.Business.Models;
using EMS.Data.Models;

namespace EMS.Business.Interfaces
{
    public interface IReviewService : IGenericCrudService<Review, ReviewModel>
    {
        Task<IEnumerable<ReviewModel>> GetReviewsByVendorIdAsync(int vendorId);
        Task<double> GetAverageRatingForEventAsync(int eventId);
        Task<double> GetAverageRatingForVendorAsync(int vendorId);
    }
}
