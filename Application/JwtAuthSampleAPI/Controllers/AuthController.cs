using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtAuthSampleAPI.Configuration;
using JwtAuthSampleAPI.Data;
using JwtAuthSampleAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthSampleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtBearerTokenSettings jwtBearerTokenSettings;
        private readonly UserManager<ApplicationUser> userManager;
        private ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context, IOptions<JwtBearerTokenSettings> jwtTokenOptions, UserManager<ApplicationUser> userManager)
        {
            this.jwtBearerTokenSettings = jwtTokenOptions.Value;
            this.userManager = userManager;
            this._context = context;
        }

        [HttpGet]
        [Route("UserInfo")]
        public async Task<IActionResult> UserInfo()
        {
            ClaimsPrincipal currentUser = User;

            var user = await _context.Users.Where(user => user.UserName == currentUser.Identity.Name).Select(
            user => new { user.Id, user.Email, user.UserName }).FirstAsync();

            return Ok(new { user });
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody]UserDetails userDetails)
        {
            if (!ModelState.IsValid || userDetails == null)
            {
                return new BadRequestObjectResult(new { Message = "User Registration Failed" });
            }

            var ApplicationUser = new ApplicationUser() { UserName = userDetails.UserName, Email = userDetails.Email };
            var result = await userManager.CreateAsync(ApplicationUser, userDetails.Password);
            if (!result.Succeeded)
            {
                var dictionary = new ModelStateDictionary();
                foreach (IdentityError error in result.Errors)
                {
                    dictionary.AddModelError(error.Code, error.Description);
                }

                return new BadRequestObjectResult(new { Message = "User Registration Failed", Errors = dictionary });
            }

            return Ok(new { Message = "User Reigstration Successful" });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody]LoginCredentials credentials)
        {
            ApplicationUser ApplicationUser;

            if (!ModelState.IsValid 
                || credentials == null
                || (ApplicationUser = await ValidateUser(credentials)) == null)
            {
                return new BadRequestObjectResult(new { Message = "Login failed" });
            }

            var token = GenerateToken(ApplicationUser);
            return Ok(new { Token = token, Message = "Success" });
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            // Well, What do you want to do here ?
            // Wait for token to get expired OR 
            // Maintain token cache and invalidate the tokens after logout method is called
            return Ok(new { Token = "", Message = "Logged Out" });
        }

        private async Task<ApplicationUser> ValidateUser(LoginCredentials credentials)
        {
            var ApplicationUser = await userManager.FindByNameAsync(credentials.Username);
            if (ApplicationUser != null)
            {
                var result = userManager.PasswordHasher.VerifyHashedPassword(ApplicationUser, ApplicationUser.PasswordHash, credentials.Password);
                return result == PasswordVerificationResult.Failed ? null : ApplicationUser;
            }

            return null;
        }


        private object GenerateToken(ApplicationUser ApplicationUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtBearerTokenSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, ApplicationUser.UserName.ToString()),
                    new Claim(ClaimTypes.Email, ApplicationUser.Email)
                }),

                Expires = DateTime.UtcNow.AddSeconds(jwtBearerTokenSettings.ExpiryTimeInSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = jwtBearerTokenSettings.Audience,
                Issuer = jwtBearerTokenSettings.Issuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}