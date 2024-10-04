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

        // Sign-up endpoint for customers.
        // POST: api/user/signup
        // Parameters: A UserModel object containing user details.
        // Returns: The signed-up user if successful, or 400 Bad Request if not.
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserModel user)
        {
            // Check if the email and password are provided.
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Email and password are required.");
            }

            // Call the service to handle sign-up.
            var result = await _userService.SignUpAsync(user);
            if (result)
            {
                return Ok(user);
            }

            return BadRequest("Sign-up failed.");
        }

        // Login endpoint.
        // POST: api/user/login
        // Parameters: UserModel containing login credentials (email and password).
        // Returns: The authenticated user if login is successful, or 401 Unauthorized if not.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserModel loginRequest)
        {
            // Call the service to authenticate the user.
            var user = await _userService.LoginAsync(loginRequest.Email, loginRequest.Password);
            if (user != null)
            {
                return Ok(user);
            }

            return Unauthorized("Invalid credentials or account not active.");
        }

        // Admin: Approve user registration.
        // PUT: api/user/admin/approve-user/{id}
        // Parameters: The user ID.
        // Returns: Success message if user is approved, or 404 Not Found if the user does not exist.
        [HttpPut("admin/approve-user/{id}")]
        public async Task<IActionResult> ApproveUser(string id)
        {
            // Call the service to approve the user registration.
            var result = await _userService.ApproveUserAsync(id);
            if (result)
            {
                return Ok("User approved successfully.");
            }

            return NotFound("User not found.");
        }

        // Admin: Reject user registration.
        // PUT: api/user/admin/reject-user/{id}
        // Parameters: The user ID.
        // Returns: Success message if the user is rejected, or 404 Not Found if the user does not exist.
        [HttpPut("admin/reject-user/{id}")]
        public async Task<IActionResult> RejectUser(string id)
        {
            // Call the service to reject the user registration.
            var result = await _userService.RejectUserAsync(id);
            if (result)
            {
                return Ok("User rejected successfully.");
            }

            return NotFound("User not found.");
        }

        // Admin: Create a new user.
        // POST: api/user/admin/createUser
        // Parameters: A UserModel object containing user details.
        // Returns: The created user with a status of 201 Created.
        [HttpPost("admin/createUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserModel user)
        {
            // Call the service to create a new user.
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        // Retrieve a user by ID.
        // GET: api/user/getUserById/{id}
        // Parameters: The user ID.
        // Returns: The user details if found, or 404 Not Found if the user does not exist.
        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            // Call the service to retrieve the user by ID.
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // Update a user's account.
        // PUT: api/user/updateUser/{id}
        // Parameters: The user ID and the updated user details.
        // Returns: A success message if the user is updated, or 404 Not Found if the user does not exist.
        [HttpPut("updateUser/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserModel updatedUser)
        {
            // Check if the user exists before updating.
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Call the service to update the user.
            var result = await _userService.UpdateUserAsync(id, updatedUser);
            if (result)
            {
                return Ok("User account updated successfully.");
            }

            return BadRequest("Failed to update user account.");
        }

        // Deactivate a user's account.
        // PUT: api/user/{id}/deactivateUser
        // Parameters: The user ID.
        // Returns: A success message if deactivated, or 404 Not Found if the user does not exist.
        [HttpPut("deactivateUser")]
        public async Task<IActionResult> DeactivateAccount([FromBody] string email)
        {
            // Check if the user exists before deactivating, using email only
            var user = await _userService.GetUserByEmailAsync(email); // Use a dummy password to check if user exists
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Ensure only customers can deactivate their own accounts.
            if (user.Role != "Customer")
            {
                return Forbid("Only customers can deactivate their own account.");
            }

            // Call the service to deactivate the account.
            var result = await _userService.DeactivateUserAccountAsync(email);
            if (result)
            {
                return Ok("User account deactivated successfully.");
            }

            return BadRequest("Failed to deactivate user account.");
        }

        // Reactivate a user's account (CSR role only).
        // PUT: api/user/{id}/reactivateUser
        // Parameters: The user ID.
        // Returns: A success message if reactivated, or 404 Not Found if the user does not exist.
        [HttpPut("reactivateUser")]
        public async Task<IActionResult> ReactivateAccount([FromBody] string email)
        {
            // Retrieve the user by email before reactivating.
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Ensure only customers can reactivate their own accounts.
            if (user.Role != "Customer")
            {
                return Forbid("Only customers can reactivate their own account.");
            }

            // Call the service to reactivate the account.
            var result = await _userService.ReactivateUserAccountAsync(email);
            if (result)
            {
                return Ok("User account reactivated successfully.");
            }

            return BadRequest("Failed to reactivate user account.");
        }


        // Retrieve all users from the system.
        // GET: api/user/getAllUsers
        // Returns: A list of all users in the database.
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            // Call the service to retrieve all users.
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}
