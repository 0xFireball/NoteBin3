using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using NoteBin3.Models.Auth;

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
                return "Login not implemented.";
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