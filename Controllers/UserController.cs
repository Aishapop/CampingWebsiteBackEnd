using Camping.Entities;
using Camping.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Cors;
using System.Text.Json;

namespace Camping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class UserController : ControllerBase
    {
        private readonly DatabaseAccess _databaseAccess;
        private readonly ILogger<UserController> _logger;

        public UserController(DatabaseAccess databaseAccess, ILogger<UserController> logger)
        {
            _logger = logger;
            _databaseAccess = databaseAccess;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _databaseAccess.GetAllUsers();
            if (users == null)
            {
                return NotFound(new { message = $"No users found" });
            }

            return users;
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetByEmail(string email)
        {
            var user = await _databaseAccess.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound(new { message = $"User with Email: {email} not found" });
            }

            return Ok(user);
        }

        [HttpGet("id/{id}")]
        public async Task<ActionResult<User>> GetByUserID(int id)
        {
            var user = await _databaseAccess.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID: {id} not found" });
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult> AddUser([FromBody] User newUser)
        {
            await _databaseAccess.AddUser(newUser);
            return CreatedAtAction(nameof(GetByEmail), new { email = newUser.Email, pw = newUser.Password }, newUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {

                var user = await _databaseAccess.GetUserByEmail(request.Email);
                _logger.LogInformation("Login request received for email: {Email}", request.Email);

                if (user == null)
                {
                    Console.WriteLine($"User with email {request.Email} not found.");
                    return Ok(new { success = false, message = "User not found" });
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if (!isPasswordValid)
                {
                    Console.WriteLine("Invalid password");
                    return Ok(new { success = false, message = "Invalid password" });
                }



                if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    return Ok(new
                    {
                        success = true,
                        user = new
                        {
                            user.UserId,
                            user.Username,
                            user.Email,
                            user.role
                        }
                    });
                }
                else
                {
                    return Ok(new { success = false });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUserPassword(int id, [FromBody] JsonElement json)
        {
            try
            {
                if (json.TryGetProperty("Password", out JsonElement passwordElement) && passwordElement.ValueKind == JsonValueKind.String)
                {
                    string newPassword = passwordElement.GetString();
                    await _databaseAccess.UpdateUserPassword(id, newPassword);
                    return NoContent();
                }
                else
                {
                    return BadRequest("Password property not found or invalid in the JSON payload");
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user password");
                return StatusCode(500, "Internal server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.UserId)
            {
                return BadRequest("User ID mismatch");
            }

            var user = await _databaseAccess.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            await _databaseAccess.UpdateUser(updatedUser);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _databaseAccess.DeleteUser(id);
                return NoContent(); // HTTP 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpOptions("login")]
        public IActionResult LoginOptions()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:8085");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST");
            Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
            return Ok();
        }

        
    }
}
