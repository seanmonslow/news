using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuthSampleAPI.Models
{
    public class LogInUser
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
