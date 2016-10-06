using Nancy;

namespace NoteBin3.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", args =>
            {
                return View["Home"];
            });
        }
    }
}