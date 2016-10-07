using Nancy;
using Nancy.Authentication.Forms;
using NoteBin3.Services.Database;
using System;
using System.Security.Claims;

namespace NoteBin3.Services.Authentication
{
    public class WebLoginUserResolver : IUserMapper
    {
        public ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            RegisteredUser storedUserRecord = null;
            using (var db = new DatabaseAccessService().OpenOrCreateDefault())
            {
                var registeredUsers = db.GetCollection<RegisteredUser>(DatabaseAccessService.UsersCollectionDatabaseKey);
                var userRecord = registeredUsers.FindOne(u => u.Identifier == identifier);
                storedUserRecord = userRecord;
            }
            if (storedUserRecord == null)
            {
                return null;
            }
            var userIdentity = new ClaimsIdentity(new AuthenticatedUser
            {
                Name = storedUserRecord.Username,
                IsAuthenticated = true,
            });

            userIdentity.AddClaim(new Claim(nameof(RegisteredUser.Username), storedUserRecord.Username));

            return new ClaimsPrincipal(userIdentity);
        }

        public RegisteredUser FindUserByUsername(string username)
        {
            RegisteredUser storedUserRecord = null;
            using (var db = new DatabaseAccessService().OpenOrCreateDefault())
            {
                var registeredUsers = db.GetCollection<RegisteredUser>(DatabaseAccessService.UsersCollectionDatabaseKey);
                var userRecord = registeredUsers.FindOne(u => u.Username == username);
                storedUserRecord = userRecord;
            }
            if (storedUserRecord == null)
            {
                return null;
            }
            return storedUserRecord;
        }
    }
}