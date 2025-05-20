﻿using System.ComponentModel.DataAnnotations;

namespace RegisterStudents.API.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<User>? Users { get; set; }
    }
}
