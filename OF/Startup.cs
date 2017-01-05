using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OF.Startup))]
namespace OF
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
