using Nancy;
using Nancy.Security;

namespace NoteBin3.Modules.Dashboard
{
    public class DashboardModule : NancyModule
    {
        public DashboardModule()
        {
            this.RequiresAuthentication();
            Get("/dashboard", args =>
            {
                return View["App/Dashboard", new { LoggedIn = true }];
            });
        }
    }
}