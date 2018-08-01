using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GestCTI.Startup))]
namespace GestCTI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            // Websockets
            app.MapSignalR();
        }
    }
}
