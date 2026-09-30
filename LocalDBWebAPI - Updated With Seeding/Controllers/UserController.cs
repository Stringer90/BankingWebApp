using Microsoft.AspNetCore.Mvc;
using LocalDBWebAPI.Models;
using System.Diagnostics;
using LocalDBWebAPI.Data;
using Microsoft.AspNetCore.Http;

namespace LocalDBWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        [HttpPost]
        [Route("createuser")]
        public IActionResult CreateUser([FromBody] User user)
        {
            if (DBManager.CreateUser(user.username, user.password, user.email, user.address, user.phone))
            {
                return Ok(new { message = "Successfully created user" });
            }
            return BadRequest(new { message = "Error when creating user" });
        }


        // Retrieve user profile (by username or email) (including user_id)
        [HttpGet]
        [Route("getuser/{searchString}")]
        public IActionResult GetUser(string searchString)
        {
            User user = DBManager.GetUser(searchString);

            // if account not found, account will be null, checked when getting request results
            return Json(user);
        }

        // Update user profile
        [HttpPost]
        [Route("updateuser")]
        public IActionResult UpdateUser([FromBody] User user)
        {
            string username = user.username;
            string password = user.password;
            string email = user.email;
            string address = user.address;
            string phone = user.phone;
            if (DBManager.UpdateUser(user.username, user.password, user.email, user.address, user.phone, user.user_id))
            {
                return Ok(new { message = "Successfully updated user" });
            }
            return BadRequest(new { error = "Error when updating user" });
        }


        // Delete user profile
        [HttpPost]
        [Route("deleteuserid/{user_id}")]
        public IActionResult DeleteUser(int user_id)
        {
            if (DBManager.DeleteUser(user_id))
            {
                return Ok("Successfully deleted user");
            }
            return BadRequest("Error when deleting user");
        }


        // ===== OTHER METHODS FOR USE IN TUTORIAL 8 APP =====

        [HttpGet]
        [Route("isvalidauth/{un_email}/{password}")]
        public IActionResult IsValidAuth(string un_email, string password)
        {
            bool isValid = DBManager.IsValidAuth(un_email, password);
            if (isValid)
            {
                // Set the session after successful login
                HttpContext.Session.SetString("User", un_email);
            }
            return Json(isValid);
        }

        [HttpGet]
        [Route("isloggedin")]
        public IActionResult IsLoggedIn()
        {
            // Check if session has User logged in
            if (HttpContext.Session.GetString("User") != null)
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok();
        }


    }
}
