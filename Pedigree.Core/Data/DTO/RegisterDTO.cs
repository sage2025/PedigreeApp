﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class RegisterDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
