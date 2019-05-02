using BinarApp.Core.Interfaces;
using BinarApp.Jobs;
using BinarApp.Providers;
using BinarApp.Utils;
using Quartz;
using System.Windows;
using Unity;
using Unity.Lifetime;

namespace BinarApp
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            IUnityContainer container = new UnityContainer();

            // Register a default (un-named) type mapping with a singleton lifetime
            container.RegisterType<IFolderListenerProvider,
                FolderListenerProvider>(new ContainerControlledLifetimeManager());
            //container.RegisterType<INetworkUtils,
            //    NetworkUtils>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITcpListenerProvider,
                TcpListenerProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<IHttpClientProvider,
                HttpClientProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDataCacheProvider,
                DataCacheProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<IJobScheduler,
                JobScheduler>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDataCacheJobListener,
                DataCacheJobListener>(new ContainerControlledLifetimeManager());
            
            // Following code will return a singleton instance
            // Container will take over lifetime management of the object
            container.Resolve<IFolderListenerProvider>();
            container.Resolve<ITcpListenerProvider>();
            //container.Resolve<INetworkUtils>();
            container.Resolve<IHttpClientProvider>();
            container.Resolve<IDataCacheProvider>();
            container.Resolve<IJobScheduler>();
            container.Resolve<IDataCacheJobListener>();

            var mainWindow = container.Resolve<MainWindow>(); // Creating Main window
            mainWindow.Show();

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

        }
    }
}
