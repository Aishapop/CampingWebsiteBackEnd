using Camping.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Camping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class ReviewController : ControllerBase
    {
        private readonly ILogger<ReviewController> _logger;
        private readonly DatabaseAccess _databaseAccess;

        public ReviewController (ILogger<ReviewController> logger, DatabaseAccess databaseAccess)
        {
            _logger = logger;
            _databaseAccess = databaseAccess;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAll()
        {
            var reviews = await _databaseAccess.GetAllReviews();
            if (reviews == null)
            {
                return NotFound(new { message = "No reviews found" }); 
            }
            return reviews;
        }

        


        [HttpGet("/test-getreviewbyid")]
        public async Task<IActionResult> GetReviewByID()
        {
            try
            {
                
                int Id = 15;

               
                var review = await _databaseAccess.GetReviewById(Id);

                if (review == null)
                {
                    return NotFound($"Review with ID {Id} not found");
                }

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("/GetReviewsBySpotID/{SpotID}")]
        public async Task<IActionResult> GetReviewsBySpotID(int SpotID)
        {
            try
            {

                


                var review = await _databaseAccess.GetReviewsBySpotId(SpotID);

                if (review == null)
                {
                    return NotFound($"Review with ID {SpotID} not found");
                }

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddReview([FromBody] Review newReview)
        {
            await _databaseAccess.AddReview(newReview);
            return CreatedAtAction(nameof(AddReview), new { id = newReview.ReviewID }, newReview);
        }

        [HttpDelete("{Userid}")]
        public async Task<IActionResult> Deletereview(int userid)
        {
            try
            {
                await _databaseAccess.DeleteReview(userid);
                return NoContent(); // HTTP 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}
