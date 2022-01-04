﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string Token { get; set; }
    }
}
