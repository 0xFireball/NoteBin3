using LiteDB;
using LiteDB.Platform;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Session;
using Nancy.TinyIoc;
using NoteBin3.Services.Authentication;

namespace NoteBin3
{
    public class NoteBinBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("assets", "wwwroot/assets/")
            );
            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("static", "wwwroot/static/")
            );
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            CookieBasedSessions.Enable(pipelines);
            container.Register<IUserMapper, WebLoginUserManager>();

            //Initialize database
            LitePlatform.Initialize(new LitePlatformNetCore());
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);


            var formsAuthConfiguration =
            new FormsAuthenticationConfiguration()
            {
                RedirectUrl = "/login",
                UserMapper = container.Resolve<IUserMapper>(),
            };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }
    }
}