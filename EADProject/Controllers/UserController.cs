/***************************************************************
 * File Name: UserController.cs
 * Description: This controller provides API endpoints for managing 
 *              user accounts, including creating, updating, deactivating,
 *              and retrieving users. It leverages UserService for
 *              communication with MongoDB.
 * Author: Chanukya Serasinghe, Nashali Perera
 * Date Created: September 15, 2024
 * Notes: Ensure that proper authorization is applied to sensitive 
 *        operations like updating or deactivating accounts.
 ***************************************************************/

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

        // Constructor: Injects the UserService dependency.
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // Create a new user.
        // POST: api/user/create
        // Parameters: A UserModel object containing user details.
        // Returns: The created user with a status of 201 Created.
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserModel user)
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        // Retrieve a user by ID.
        // GET: api/user/{id}
        // Parameters: The user ID.
        // Returns: The user details if found, or 404 Not Found if not.
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

        // Update a user's account.
        // PUT: api/user/{id}
        // Parameters: The user ID and the updated user details.
        // Returns: A success message if updated, or 404 Not Found if user not found, or 403 Forbid if unauthorized.
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

        // Deactivate a user's account.
        // PUT: api/user/{id}/deactivate
        // Parameters: The user ID.
        // Returns: A success message if deactivated, or 404 Not Found if user not found, or 403 Forbid if unauthorized.
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

        // Reactivate a user's account (CSR role only).
        // PUT: api/user/{id}/reactivate
        // Parameters: The user ID.
        // Returns: A success message if reactivated, or 404 Not Found if user not found, or 403 Forbid if unauthorized.
        [HttpPut("{id}/reactivate")]
        public async Task<IActionResult> ReactivateAccount(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Ensure that the currently logged-in user is a CSR.
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

        // Retrieve all users from the system.
        // GET: api/user/all
        // Returns: A list of all users in the database.
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}
