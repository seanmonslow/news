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
        [Route("GetNewArticles")]
        public async Task<IActionResult> getNewArticles()
        {

            ClaimsPrincipal currentUser = User;

            var user = await ApplicationUser.GenerateUserIdentityAsync(_context, currentUser.Identity.Name);

            var articles = await _context.Article.FromSqlRaw("SELECT Article.* FROM Article WHERE Article.id NOT IN (SELECT ArticleUser.Articleid FROM ArticleUser WHERE ArticleUser.Userid = {0})", user.Id).ToListAsync();

            return Ok(new { articles });

        }

        [HttpPost]
        [Route("CreateArticleReview")]
        public async Task<IActionResult> createArticleReview([FromBody] ArticleUser model)
        {

            ClaimsPrincipal currentUser = User;

            var user = await ApplicationUser.GenerateUserIdentityAsync(_context, currentUser.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool exists = (from articleUsers in _context.ArticleUser
                           where articleUsers.User.Id == user.Id && articleUsers.Article.id == model.Article.id
                           select articleUsers).Any();

            if (exists)
            {
                return new BadRequestObjectResult(new { Message = "Already reviewed" });
            }

            var articleUser = new ArticleUser { };

            articleUser.Like = model.Like;

            ApplicationUser exisitingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);

            articleUser.User = exisitingUser;

            Article exisitingArticle = _context.Article.FirstOrDefault(a => a.id == model.Article.id);

            if(exisitingArticle == null)
            {
                return new BadRequestObjectResult(new { Message = "Article doesn't exist" });
            }

            articleUser.Article = exisitingArticle;

            _context.Add(articleUser);

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Review successfully created" });

        }

        public static implicit operator ArticleController(ApplicationDbContext v)
        {
            throw new NotImplementedException();
        }
    }
}