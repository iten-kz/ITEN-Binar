using System;
using System.Collections.Generic;
using System.Linq;
using BinarApp.API.Utils;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BinarApp.API.Startup))]

namespace BinarApp.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            ConfigureAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}
