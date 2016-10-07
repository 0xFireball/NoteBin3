using System;
using System.Security.Claims;
using Nancy;
using Nancy.Authentication.Forms;

namespace NoteBin3.Services.Authentication
{
    public class WebLoginUserMapper : IUserMapper
    {
        public ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            throw new NotImplementedException();
        }
    }
}