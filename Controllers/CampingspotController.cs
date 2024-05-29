using Camping.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Camping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class CampingspotController : ControllerBase
    {
        

        private readonly ILogger<CampingspotController> _logger;
        private readonly DatabaseAccess _databaseAccess;
        public CampingspotController(ILogger<CampingspotController> logger, DatabaseAccess databaseAccess)
        {
            _logger = logger;
            _databaseAccess = databaseAccess;
        }

        
        

        [HttpGet]
        public async Task<IEnumerable<Campingspot>>GetAll()
        {
            var campingspots = _databaseAccess.GetAllCampingspots();
            
            if (campingspots == null) return null;
            return await campingspots;
        }

        [HttpPost]
        public async Task<ActionResult> AddCampingspot([FromBody] Campingspot newCampingspot)
        {
            await _databaseAccess.AddCampingspot(newCampingspot);

            return CreatedAtAction(nameof(GetAll), new { id = newCampingspot.SpotID }, newCampingspot);
        }

        [HttpGet("/test-getcampingbyid")]
        public async Task<IActionResult> GetCampingByID()
        {
            try
            {
                
                int Id = 1;

                
                var camping = await _databaseAccess.GetCampingspotById(Id);

                if (camping == null)
                {
                    return NotFound($"camping with ID {Id} not found");
                }

                return Ok(camping);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{Ownerid}")]
        public async Task<IActionResult> DeleteCampingspot(int id)
        {
            try
            {
                // Check if the user has booking
                var bookins = await _databaseAccess.GetBookingsByUserId(id);
                if (bookins.Any())
                {
                    await _databaseAccess.DeleteBooking(id);
                }

                await _databaseAccess.DeleteCampingspot(id);
                return NoContent(); // HTTP 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}
