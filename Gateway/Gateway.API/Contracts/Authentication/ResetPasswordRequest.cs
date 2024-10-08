﻿using System.ComponentModel.DataAnnotations;

namespace Gateway.API.Contracts.Authentication
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Token is required.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

    }
}
