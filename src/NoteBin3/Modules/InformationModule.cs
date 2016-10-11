using Nancy;
using Nancy.Responses;

namespace NoteBin3.Modules
{
    public class InformationModule : NancyModule
    {
        public InformationModule()
        {
            Get("/tos", args =>
            {
                return View["Tos"];
            });
            Get("/about", args =>
            {
                return View["About"];
            });
            Get("/contact", args =>
            {
                return new RedirectResponse("https://0xfireball.me");
            });
        }
    }
}