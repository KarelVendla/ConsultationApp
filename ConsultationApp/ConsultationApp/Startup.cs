using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ConsultationApp.Startup))]
namespace ConsultationApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
