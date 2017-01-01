using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IlvtuManage.Startup))]
namespace IlvtuManage
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
