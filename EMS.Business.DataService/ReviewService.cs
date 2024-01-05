using AutoMapper;
using EMS.Business.Interfaces;
using EMS.Business.Models;
using EMS.Data.Models;

namespace EMS.Business.DataService
{
    public class ReviewService : GenericCrudService<Review, ReviewModel>, IReviewService
    {
        public ReviewService(Data.Interfaces.IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<double> GetAverageRatingForEventAsync(int eventId)
        {
            var reviewsForEvent = await _unitOfWork.GetRepository<Review>()
           .GetAll();

            var reviewsFilteredByEvent = reviewsForEvent.Where(r => r.EventId == eventId);

            if (reviewsFilteredByEvent.Any())
            {
                var avgRating = reviewsFilteredByEvent.Average(r => r.Rating);
                return Math.Round(avgRating, 2);
            }

            // case where there are no reviews for the event.
            return 0.0;
        }

        public async Task<double> GetAverageRatingForVendorAsync(int vendorId)
        {
            var reviewsForVendor = await _unitOfWork.GetRepository<Review>().GetAll();

            var reviewsFilteredByVendor = reviewsForVendor.Where(r => r.VendorId == vendorId);
            if (reviewsFilteredByVendor.Any())
            {
                var avgRating = reviewsFilteredByVendor.Average(r => r.Rating);
                return Math.Round(avgRating, 2);
            }

            // case where there are no reviews for the event.
            return 0.0;
        }

        public async Task<IEnumerable<ReviewModel>> GetReviewsByVendorIdAsync(int vendorId)
        {
            var reviewsForVendor = await _unitOfWork.GetRepository<Review>()
                .GetAll();
            var reviewFilterForVendor = reviewsForVendor.Where(r => r.VendorId == vendorId);

            var reviewModels = _mapper.Map<IEnumerable<ReviewModel>>(reviewFilterForVendor);
            return reviewModels;
        }

    }
}
