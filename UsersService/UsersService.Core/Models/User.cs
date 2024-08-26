﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommerceFlow.Persistence.Entities
{
    public class User
    {
        public ulong Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string HashPassword { get; set; } = string.Empty;
    }
}
