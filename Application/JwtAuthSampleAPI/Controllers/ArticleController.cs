using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JwtAuthSampleAPI.Data;
using JwtAuthSampleAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace JwtAuthSampleAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ArticleController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public ArticleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            this._context = context;
            this._userManager = userManager;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("new")]
        public async Task<IActionResult> New()
        {

            ClaimsPrincipal currentUser = User;

            var user = await _context.Users.Where(user => user.UserName == currentUser.Identity.Name).Select(
            user => new { user.Id, user.Email, user.UserName }).FirstAsync();

            return Ok(new { user });

        }

        [HttpGet]
        [Route("NewArticles")]
        public async Task<IActionResult> getNewsArticles()
        {

            ClaimsPrincipal currentUser = User;

            var user = await ApplicationUser.GenerateUserIdentityAsync(_context, currentUser.Identity.Name);

            var articles = await _context.Article.FromSqlRaw("SELECT Article.* FROM Article WHERE Article.id NOT IN (SELECT ArticleUser.Articleid FROM ArticleUser WHERE ArticleUser.Userid = {0})", user.Id).ToListAsync();

            return Ok(new { articles });

        }

        /*public async Task<IActionResult> createArticleReview()
        {
            return Ok(new { "hello" });
        }*/

        public static implicit operator ArticleController(ApplicationDbContext v)
        {
            throw new NotImplementedException();
        }
    }
}