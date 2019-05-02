/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:BinarApp.DesktopClient"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using BinarApp.Core.Interfaces;
using BinarApp.Core.POCO;
using BinarApp.DesktopClient.Jobs;
using BinarApp.DesktopClient.Managers;
using BinarApp.DesktopClient.Models;
using BinarApp.DesktopClient.Providers;
using BinarApp.DesktopClient.Views.Pages;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using NLog;

namespace BinarApp.DesktopClient.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<IProxyService<Equipment>, EquipmentProxyService>();
            SimpleIoc.Default.Register<IProxyService<Fixation>, FixationProxyService>();
            SimpleIoc.Default.Register<FileManager>();
            SimpleIoc.Default.Register<BinarCameraManager>();
            SimpleIoc.Default.Register<TattileCameraManager>();

            SimpleIoc.Default.Register<IFolderListenerProvider, FileSystemWatcherProvider>();
            SimpleIoc.Default.Register<ITcpListenerProvider, TcpListenerProvider>();
            SimpleIoc.Default.Register<IDataCacheProvider, DataCacheProvider>();
            SimpleIoc.Default.Register<IJobScheduler, JobScheduler>();
            SimpleIoc.Default.Register<IDataCacheJobListener, DataCacheJobListener>();

            SimpleIoc.Default.Register<EmployeePlateNumberService>();

            SimpleIoc.Default.Register<PlateNumberViewModel>();
            SimpleIoc.Default.Register<MainPageView>();
            SimpleIoc.Default.Register<SearchPageView>();
            SimpleIoc.Default.Register<FormPageView>();
            SimpleIoc.Default.Register<VideoPageView>();
            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SearchPageViewModel>();
            SimpleIoc.Default.Register<FormPageViewModel>();
            SimpleIoc.Default.Register<ParkingViewModel>();
            SimpleIoc.Default.Register<ParkingPageView>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}