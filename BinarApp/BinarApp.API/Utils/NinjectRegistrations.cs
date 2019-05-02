using BinarApp.API.Models;
using BinarApp.Core.Provider;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace BinarApp.API.Utils
{
    public class NinjectRegistrations : NinjectModule
    {
        public override void Load()
        {
            Bind<BlobStorageProvider>().To<BlobStorageProvider>();            
        }
    }
}