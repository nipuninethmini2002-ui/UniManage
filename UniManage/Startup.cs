using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UniManage.Startup))]
namespace UniManage
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
