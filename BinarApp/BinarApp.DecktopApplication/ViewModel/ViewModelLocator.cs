/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:BinarApp.DecktopApplication"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using BinarApp.Core.POCO;
using BinarApp.DecktopApplication.Models;
using BinarApp.DecktopApplication.Proxies;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace BinarApp.DecktopApplication.ViewModel
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
            
            SimpleIoc.Default.Register<CsvReader, CsvReader>();
            SimpleIoc.Default.Register<FileProvider, FileProvider>();
            SimpleIoc.Default.Register<GeoCoordinateManager, GeoCoordinateManager>();
            SimpleIoc.Default.Register<LocationManager, LocationManager>();
            SimpleIoc.Default.Register<ProxyService<Equipment>, ProxyService<Equipment>>();
            SimpleIoc.Default.Register<ProxyService<Fixation>, ProxyService<Fixation>>();
            SimpleIoc.Default.Register<FixationIncidentProxy, FixationIncidentProxy>();
            SimpleIoc.Default.Register<CsvDetailReader, CsvDetailReader>();
            SimpleIoc.Default.Register<FileDetailProvider, FileDetailProvider>();

            SimpleIoc.Default.Register<MainViewModel>();
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