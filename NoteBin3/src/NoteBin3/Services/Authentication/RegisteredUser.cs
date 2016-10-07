using System;

namespace NoteBin3.Services.Authentication
{
    public class RegisteredUser
    {
        public string Username { get; set; }
        
        public string Password { get; set; }

        public Guid Identifier { get; set; }
    }
}