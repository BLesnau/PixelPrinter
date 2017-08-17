using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PixelPrinterService.Startup))]

namespace PixelPrinterService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}