using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using Nancy.Responses;
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
                if (Context.CurrentUser != null)
                {
                    return new RedirectResponse("./dashboard/");
                }
                return View["Login"];
            });
            
            //When the login form is submitted
            Post("/login", args =>
            {
                var loginParams = this.Bind<WebLoginParams>();

                var userManagerConnection = new WebLoginUserManager();
                var matchingUser = userManagerConnection.FindUserByUsername(loginParams.Username);

                if (matchingUser == null || !userManagerConnection.CheckPassword(loginParams.Password, matchingUser))
                {
                    //return "Invalid login credentials!";
                    return View["Login", new { LoginError = true }];
                }

                var expiryTime = DateTime.Now.AddDays(1);
                return this.LoginAndRedirect(matchingUser.Identifier, expiryTime, "/dashboard/");
            });

            //Signup form
            Get("/signup", args =>
            {
                if (Context.CurrentUser != null)
                {
                    return new RedirectResponse("/dashboard/");
                }
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
                    var userManagerConnection = new WebLoginUserManager();
                    //This can take a bit of time, as crypto stuff has to be generated
                    userManagerConnection.RegisterUser(signupParams);
                }
                catch
                {
                    return View["Signup", new { SignupError = true, ErrorMessage = ": username is taken!" }];
                }

                return View["Login", new { ToContinue = true }];
            });

            //Logout
            Get("/logout", args =>
            {
                //Log out the user
                return this.LogoutAndRedirect("/home");
            });
        }
    }
}