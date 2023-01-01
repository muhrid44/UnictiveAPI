using EntityModel;
using HelperClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServiceInterface;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using Utility;
using static System.Net.WebRequestMethods;

namespace UnictiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        //creating a user
        [HttpPost("register")]
        public async Task<ActionResult<UserAuthModel>> Register([FromBody] UserModel model)
        {

            var result = await _userService.Create(model);

            return Ok(result);
            
        }

        //login to get JWT saved into cookies
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserModel model)
        {
            bool IsEmailExist = await _userService.EmailExist(model);

            if(!IsEmailExist)
            {
                return BadRequest(NotificationModel.ErrorMessage);
            }

            bool IsPasswordVerified = await _userService.PasswordVerification(model);

            if(!IsPasswordVerified)
            {
                return BadRequest("Wrong password");
            }

            HttpContext.Response.Cookies.Append("SecretToken", ApplicationSettings.TokenLogin, new CookieOptions { Expires = DateTime.Now.AddDays(1), HttpOnly = true });

            return Ok(ApplicationSettings.TokenLogin);
        }

        //logout to remove JWT from cookies
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Delete("SecretToken");
            return Redirect("/swagger/index.html");
        }

        //read user by token JWT
        [HttpGet("user")]
        public async Task<ActionResult<UserModel>> GetUser()
        {
            HttpContext.Request.Cookies.TryGetValue("SecretToken", out string token);

            if(token == null)
            {
                return BadRequest("Please login first!");
            }

            var userEmail = JWTUtility.GetEmailToken(token);

            var userProfile = await _userService.GetUserProfile(userEmail);

            return Ok(userProfile);
        }

        //update user by token JWT
        [HttpPut("edit-user")]
        public async Task<IActionResult> Update([FromBody] UserModel model)
        {
            HttpContext.Request.Cookies.TryGetValue("SecretToken", out string token);

            if (token == null)
            {
                return BadRequest("Please login first!");
            }

            var userEmail = JWTUtility.GetEmailToken(token);

            var userProfile = await _userService.GetUserProfile(userEmail);

            var result = await _userService.Update(userProfile, model);

            HttpContext.Response.Cookies.Delete("SecretToken");

            return Ok(result);
        }

        //adding hobby explicitly
        [HttpPost("add-hobby")]
        public async Task<IActionResult> AddHobby([FromBody] string hobby)
        {
            HttpContext.Request.Cookies.TryGetValue("SecretToken", out string token);

            if (token == null)
            {
                return BadRequest("Please login first!");
            }

            var userEmail = JWTUtility.GetEmailToken(token);

            var userProfile = await _userService.GetUserProfile(userEmail);

            var result = await _userService.AddHobby(userProfile, hobby);

            return Ok(result);

        }

        //delete hobby explicitly
        [HttpPost("delete-hobby")]
        public async Task<IActionResult> DeleteHobby([FromBody] string hobby)
        {
            HttpContext.Request.Cookies.TryGetValue("SecretToken", out string token);

            if (token == null)
            {
                return BadRequest("Please login first!");
            }

            var userEmail = JWTUtility.GetEmailToken(token);

            var userProfile = await _userService.GetUserProfile(userEmail);

            var result = await _userService.DeleteHobby(userProfile, hobby);

            return Ok(result);

        }
    }
}
