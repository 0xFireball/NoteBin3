using Nancy;

namespace KJade.ViewEngine.Demo.Modules
{
    public class TestPageModule : NancyModule
    {
        public TestPageModule()
        {
            Get("/test", args =>
            {
                var model = new { Name = "Bob" };
                return View["Test", model];
            });
        }
    }
}