using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using NoteBin3.Models.Auth;
using NoteBin3.Services.Authentication;
using System;

namespace NoteBin3.Modules
{
    public class AuthModule : NancyModule
    {
        public AuthModule()
        {
            //Login form
            Get("/login", args =>
            {
                return View["Login"];
            });

            //When the login form is submitted
            Post("/login", args =>
            {
                var loginParams = this.Bind<WebLoginParams>();

                //Check password (CURRENTLY INSECURE!)
                var dbConnection = new WebLoginUserManager();
                var matchingUser = dbConnection.FindUserByUsername(loginParams.Username);

                if (matchingUser == null || matchingUser.Password != loginParams.Password)
                {
                    //return "Invalid login credentials!";
                    return View["Login", new { LoginError = true }];
                }

                var expiryTime = DateTime.Now.AddDays(1);
                return this.LoginAndRedirect(matchingUser.Identifier, expiryTime, "./dashboard");
            });

            //Signup form
            Get("/signup", args =>
            {
                return View["Signup"];
            });

            Post("/signup", args =>
            {
                var signupParams = this.Bind<WebSignupParams>();

                //Validate request

                //Make sure passwords match!
                if (signupParams.Password != signupParams.ConfirmPassword)
                {
                    return View["Signup", new { SignupError = true, ErrorMessage = ": confirmation does not match password" }];
                }
                //Make sure they accepted
                if (!signupParams.IUnderstand || !signupParams.IAccept)
                {
                    return View["Signup", new { SignupError = true, ErrorMessage = ": you must accept the terms of service and disclaimer!" }];
                }

                //Store account in database! First check for conflicts and stuff
                //Creation should throw a security exception
                try
                {
                    var dbConnection = new WebLoginUserManager();
                    dbConnection.RegisterUser(signupParams);
                }
                catch
                {
                    return View["Signup", new { SignupError = true, ErrorMessage = ": username is taken!" }];
                }

                //TODO: Calculate a secure hash of the password and store instead of the actual password!

                return View["Login", new { ToContinue = true }];
            });

            //Logout
            Get("/logout", args =>
            {
                //Log out the user
                return this.LogoutAndRedirect("./home");
            });
        }
    }
}