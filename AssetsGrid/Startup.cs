using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AssetsGrid.Startup))]
namespace AssetsGrid
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
