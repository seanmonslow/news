using System;

namespace JwtAuthSampleAPI.Models
{
    public class Article
    {
        public int id { get; set; }
        public string author { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string urlToImage { get; set; }
        public DateTime publishedAt { get; set; }
        public string content { get; set; }
    }
}