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
                var dbConnection = new WebLoginUserResolver();
                var matchingUser = dbConnection.FindUserByUsername(loginParams.Username);

                if (matchingUser == null || matchingUser.Password != loginParams.Password)
                {
                    return "Invalid login credentials!";
                }

                var expiryTime = DateTime.Now.AddDays(1);
                return this.LoginAndRedirect(matchingUser.Identifier, expiryTime, "./dashboard");
            });

            //Signup form
            Get("/signup", args =>
            {
                return View["Signup"];
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