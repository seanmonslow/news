using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuthSampleAPI.Models
{
    [NotMapped]
    public class LoginCredentials
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
