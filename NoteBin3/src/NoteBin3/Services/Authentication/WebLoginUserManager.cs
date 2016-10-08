using Nancy;
using Nancy.Authentication.Forms;
using NoteBin3.Models.Auth;
using NoteBin3.Services.Database;
using System;
using System.Collections;
using System.Security;
using System.Security.Claims;

namespace NoteBin3.Services.Authentication
{
    public class WebLoginUserManager : IUserMapper
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

        /// <summary>
        /// Attempts to register a new user. Only the username is validated, it is expected that other fields have already been validated!
        /// </summary>
        /// <param name="signupParams"></param>
        /// <returns></returns>
        public RegisteredUser RegisterUser(WebSignupParams signupParams)
        {
            RegisteredUser newUserRecord = null;
            if (FindUserByUsername(signupParams.Username) != null)
            {
                //BAD! Another conflicting user exists!
                throw new SecurityException("A user with the same username already exists!");
            }
            using (var db = new DatabaseAccessService().OpenOrCreateDefault())
            {
                var registeredUsers = db.GetCollection<RegisteredUser>(DatabaseAccessService.UsersCollectionDatabaseKey);
                //Calculate cryptographic info
                var cryptoConf = PasswordCryptoConfiguration.CreateDefault();
                var pwSalt = AuthCryptoHelper.GetRandomSalt(64);
                var encryptedPassword = AuthCryptoHelper.CalculateUserPasswordHash(signupParams.Password, pwSalt, cryptoConf);
                //Create user
                newUserRecord = new RegisteredUser
                {
                    Identifier = Guid.NewGuid(),
                    Username = signupParams.Username,
                    CryptoSalt = pwSalt,
                    PasswordCryptoConf = cryptoConf,
                    PasswordKey = encryptedPassword,
                };
                //Add the user to the database
                registeredUsers.Insert(newUserRecord);
            }
            return newUserRecord;
        }

        public bool CheckPassword(string password, RegisteredUser userRecord)
        {
            //Calculate hash and compare
            var pwKey = AuthCryptoHelper.CalculateUserPasswordHash(password, userRecord.CryptoSalt, userRecord.PasswordCryptoConf);
            return StructuralComparisons.StructuralEqualityComparer.Equals(pwKey, userRecord.PasswordKey);
        }
    }
}