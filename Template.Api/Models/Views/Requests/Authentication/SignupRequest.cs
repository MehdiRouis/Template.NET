using System.ComponentModel.DataAnnotations;

namespace Template.Api.Models.Views.Requests.Authentication
{
    public class SignupRequest
    {
        public required string FullName { get; set; }

        public required string Phone { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }

}
