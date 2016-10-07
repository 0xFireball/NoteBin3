using System.Collections.Generic;

namespace NoteBin3.Services.Authentication
{
    public class AuthenticatedUser
    {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}