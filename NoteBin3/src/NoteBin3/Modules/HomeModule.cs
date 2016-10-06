using Nancy;

namespace NoteBin3.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", args => SendHomePage(args));
            Get("/home", args => SendHomePage(args));
        }

        private object SendHomePage(dynamic args)
        {
            return View["Home"];
        }
    }
}