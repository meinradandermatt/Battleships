using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(Battleships.Api.Startup))]
namespace Battleships.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            //app.UseCors(CorsOptions.AllowAll);

            // required to support CORS request for signalR
            app.Map("/signalr",
                map =>
                {
                    map.UseCors(CorsOptions.AllowAll);
                    var hubConfiguration = new HubConfiguration();
                    map.RunSignalR(hubConfiguration);
                });
        }
    }
}
