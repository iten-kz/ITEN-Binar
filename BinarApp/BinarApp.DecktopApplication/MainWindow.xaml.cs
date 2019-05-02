using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BinarApp.DecktopApplication
{
    using BinarApp.Core.POCO;
    using BinarApp.DecktopApplication.Models;
    using BinarApp.DecktopApplication.Proxies;
    using CommonServiceLocator;
    using Microsoft.Maps.MapControl.WPF;
    using System.Configuration;
    using System.Device.Location;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GeoCoordinateManager _geoCoordinateManager;
        private ProxyService<Equipment> _equipmentProxyService;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            _geoCoordinateManager = ServiceLocator.Current.GetInstance<GeoCoordinateManager>();
            _equipmentProxyService = ServiceLocator.Current.GetInstance<ProxyService<Equipment>>();

            _geoCoordinateManager.GeoCoordinateChanged += _geoCoordinateManager_GeoCoordinateChanged;

            GetEquipments();
        }

        private void _geoCoordinateManager_GeoCoordinateChanged(object sender, GeoCoordinate e)
        {
            var pushPin = new Pushpin()
            {
                Location = new Location(e.Latitude, e.Longitude)
            };

            foreach (var elem in mp_main.Children)
            {
                if (elem is Pushpin)
                {
                    mp_main.Children.Remove(elem as Pushpin);
                    break;
                }
            }

            mp_main.Children.Add(pushPin);
            mp_main.Center = new Location(e.Latitude, e.Longitude);
            mp_main.ZoomLevel = 14;
        }
        

        private async Task GetEquipments()
        {
            var equips = await _equipmentProxyService.GetCollection("Equipments");

            var equipPolygins = equips
                .Where(x => !string.IsNullOrEmpty(x.GeoJson))    
                .Select(x => new EquipmentPolygon(x))
                .Select(x => new MapPolygon()
                {
                    Fill = new SolidColorBrush(Colors.Blue),
                    Stroke = new SolidColorBrush(Colors.Green),
                    Opacity = 0.4,
                    StrokeThickness = 1,
                    Locations = new LocationCollection()
                    {
                        new Location(x.BottomLeft.Lat, x.BottomLeft.Lng),
                        new Location(x.TopRight.Lat, x.BottomLeft.Lng),
                        new Location(x.TopRight.Lat, x.TopRight.Lng),
                        new Location(x.BottomLeft.Lat, x.TopRight.Lng),
                    }
                }).ToList();

            mp_main.Children.Clear();

            foreach (var elem in equipPolygins)
            {
                mp_main.Children.Add(elem);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _geoCoordinateManager.GeoCoordinateChanged -= _geoCoordinateManager_GeoCoordinateChanged;

            if (_geoCoordinateManager != null)
            {
                _geoCoordinateManager.Dispose();
                _geoCoordinateManager = null;
            }
        }
    }
}
