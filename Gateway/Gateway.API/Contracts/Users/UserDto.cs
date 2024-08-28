using System.ComponentModel.DataAnnotations;

namespace Gateway.API.Contracts.Users
{
    public class UserDto
    {
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
