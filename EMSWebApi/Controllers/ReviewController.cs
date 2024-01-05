using AutoMapper;
using EMS.Business.Interfaces;
using EMS.Business.Models;
using EMS.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMSWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ReviewController(IReviewService reviewService, IMapper mapper, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("average-rating-for-event/{eventId}")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<IActionResult> GetAverageRatingForEvent(int eventId)
        {
            try
            {
                var averageRating = await _reviewService.GetAverageRatingForEventAsync(eventId);
                return Ok(new { AverageRating = averageRating });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet("average-rating-for-vendor/{vendorId}")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<IActionResult> GetAverageRatingForVendor(int vendorId)
        {
            try
            {
                var averageRating = await _reviewService.GetAverageRatingForVendorAsync(vendorId);
                return Ok(new { AverageRating = averageRating });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet("reviews-by-vendor/{vendorId}")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<IActionResult> GetReviewsByVendorId(int vendorId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByVendorIdAsync(vendorId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }


        // GET: api/Review/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            _logger.LogInformation("Attempting to retrieve review with ID: {reviewID}", id);

            var review = await _reviewService.GetByIdAsync(id);

            if (review == null)
            {
                _logger.LogError("Review Not found with specified ID: {reviewId}", id);
                return NotFound(new { message = "Review not found with specified ID" });
            }

            var reviewEntity = _mapper.Map<Review>(review);
            _logger.LogInformation("Successfully ran GetReview");
            return Ok(new { message = "Successfully Retrieved", review = reviewEntity });
        }

        //POST: api/Review
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewModel reviewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value!.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                return BadRequest(new { message = "Validation Failed", errors });
            }

            var review = _mapper.Map<Review>(reviewModel);
            await _reviewService.CreateAsync(reviewModel);
            var createdReviewModel = _mapper.Map<Review>(review);
            return Ok(new { message = "Review Created" });
        }

        [Authorize(Roles = "Administrator")]
        // PUT: api/Review/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewModel updatedReviewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(e => e.Value!.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                            );

                    return BadRequest(new { message = "Validation Failed", errors });
                }
                if (updatedReviewModel == null)
                {
                    return BadRequest(new { message = "Invalid Review data" });
                }

                var existingReview = await _reviewService.GetByIdAsync(id);

                existingReview.Comment = updatedReviewModel.Comment;
                existingReview.Rating = updatedReviewModel.Rating;
                existingReview.UserId = updatedReviewModel.UserId;
                existingReview.EventId = updatedReviewModel?.EventId;
                existingReview.VendorId = updatedReviewModel?.VendorId;

                await _reviewService.UpdateAsync(id, existingReview);

                return Ok(new { message = " Review Updated Successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred with ID: {reviewId}, Error: {Error}", id, ex.Message);
                return StatusCode(500, new { message = "An error occurred while updating the review", error = ex.Message });
            }
        }

        //DELETE: api/Review/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null)
            {
                return NotFound(new { message = "{id} Not found!", id });
            }
            await _reviewService.DeleteAsync(id);
            return Ok(new { message = "Deleted Successfully" });
        }
    }
}
