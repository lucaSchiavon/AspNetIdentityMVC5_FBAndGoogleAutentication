using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FbAndGoogleAutentication.Startup))]
namespace FbAndGoogleAutentication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
