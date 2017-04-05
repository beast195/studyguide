using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StudyGuide.Startup))]
namespace StudyGuide
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
