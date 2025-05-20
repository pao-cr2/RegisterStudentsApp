﻿namespace RegisterStudents.API.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; } // FK

        public Role? Role { get; set; }
    }
}
