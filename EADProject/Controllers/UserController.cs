using EADProject.Models;
using EADProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // Create user
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserModel user)
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        // Get user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // Update user account
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserModel updatedUser)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            if (existingUser.Role != "Customer")
            {
                return Forbid("Only customers can update their own account details.");
            }

            var result = await _userService.UpdateUserAsync(id, updatedUser);
            if (result)
            {
                return Ok("User account updated successfully.");
            }

            return BadRequest("Failed to update user account.");
        }

        // Deactivate user account
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateAccount(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Role != "Customer")
            {
                return Forbid("Only customers can deactivate their own account.");
            }

            var result = await _userService.DeactivateUserAccountAsync(id);
            if (result)
            {
                return Ok("User account deactivated successfully.");
            }

            return BadRequest("Failed to deactivate user account.");
        }

        // Reactivate user account (only CSR can reactivate)
        [HttpPut("{id}/reactivate")]
        public async Task<IActionResult> ReactivateAccount(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated && User.IsInRole("CSR"))
            {
                var result = await _userService.ReactivateUserAccountAsync(id);
                if (result)
                {
                    return Ok("User account reactivated successfully.");
                }

                return BadRequest("Failed to reactivate user account.");
            }

            return Forbid("Only CSR can reactivate deactivated accounts.");
        }

        // Get all users
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}
