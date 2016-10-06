using Nancy;

namespace NoteBin3.Modules
{
    public class AuthModule : NancyModule
    {
        public AuthModule()
        {
            Get("/login", args =>
            {
                return View["Login"];
            });

            Get("/signup", args =>
            {
                return View["Signup"];
            });

            Post("/login", args =>
            {
                return "Login not implemented.";
            });
        }
    }
}