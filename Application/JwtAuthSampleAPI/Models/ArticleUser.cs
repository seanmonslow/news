using System;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthSampleAPI.Models
{
    public class ArticleUser
    {
        public int id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Article Article { get; set; }
        public int Like { get; set; }
    }
}
