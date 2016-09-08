using Microsoft.Owin;
using Owin;

namespace Foogle
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}