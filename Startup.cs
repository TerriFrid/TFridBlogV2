using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TFridBlog.Startup))]
namespace TFridBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
