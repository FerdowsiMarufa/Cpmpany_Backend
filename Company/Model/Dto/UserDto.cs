﻿using Microsoft.AspNetCore.Identity;

namespace Company.Model.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
            
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
