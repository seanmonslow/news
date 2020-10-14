using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JwtAuthSampleAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthSampleAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<ArticleUser> ArticleUsers { get; set; }

        public static async Task<LogInUser> GenerateUserIdentityAsync(ApplicationDbContext context, string userName)
        {
            var user = await context.Users.Where(user => user.UserName == userName).Select(
            user => new LogInUser { Id = user.Id, Email = user.Email, UserName = user.UserName }).FirstAsync();

            return user;
        }
    }
}
