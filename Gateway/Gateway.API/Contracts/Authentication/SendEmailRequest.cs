using System.ComponentModel.DataAnnotations;

namespace Gateway.API.Contracts.Authentication
{
    public class SendEmailRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
    }
}
