[assembly: HostingStartup(typeof(APRP.Web.Areas.Identity.IdentityHostingStartup))]
namespace APRP.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}