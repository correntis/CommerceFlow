using System.ComponentModel.DataAnnotations;

namespace Gateway.API.Contracts.Users
{
    public class UserDto
    {
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        
        public string Phone { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}

