using Camping.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Camping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly DatabaseAccess _databaseAccess;

        public BookingController(ILogger<BookingController> logger,DatabaseAccess databaseAccess)
        {
            _logger = logger;
            _databaseAccess = databaseAccess;
        }



        [HttpGet]
        public async Task<IEnumerable<Booking>> GetAll()
        {
            var bookings = await _databaseAccess.GetAllBookings();
            if (bookings.Count == 0)
            {
                return null;
            }
            return bookings;
        }

        [HttpPost]
        public async Task<ActionResult> AddBooking([FromBody] Booking newBooking)
        {
            await _databaseAccess.AddBooking(newBooking);
            return CreatedAtAction(nameof(AddBooking), new { id = newBooking.BookingID }, newBooking);
        }

        [HttpGet("getbookingbyUserid{UserId}")]
        public async Task<IActionResult> GetBookinByID(int UserId)
        {
            try
            {
                // Assuming you want to get the review with ID 1
                

                // Call the GetReviewById method
                var booking = await _databaseAccess.GetBookingsByUserId(UserId);

                if (booking == null)
                {
                    return NotFound($"Review with ID {UserId} not found");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{UserId}/{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int UserId, int bookingId)
        {
            try
            {
                // Check if the user has reviews
                var reviews = await _databaseAccess.GetReviewsByUserId(UserId);
                if (reviews.Any())
                {
                    await _databaseAccess.DeleteReview(UserId);
                }

                await _databaseAccess.DeleteBookingByBookingId(bookingId);
                return NoContent(); // HTTP 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        
    }
}
