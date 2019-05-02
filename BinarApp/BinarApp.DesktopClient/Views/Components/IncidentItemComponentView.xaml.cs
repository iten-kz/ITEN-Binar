using GMap.NET;
using GMap.NET.WindowsPresentation;
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

namespace BinarApp.DesktopClient.Views.Components
{
    /// <summary>
    /// Логика взаимодействия для IncidentItemComponentView.xaml
    /// </summary>
    public partial class IncidentItemComponentView : UserControl
    {
        public IncidentItemComponentView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MapControl.IsManipulationEnabled = true;
            MapControl.MouseWheelZoomEnabled = true;
            MapControl.ShowCenter = false;
            MapControl.Zoom = 12;
            MapControl.Position = new PointLatLng(43.238949, 76.889709);
            MapControl.IsTextSearchEnabled = true;
            MapControl.TouchEnabled = true;

            MapControl.MapProvider = GMap.NET.MapProviders.YandexMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            var marker = new GMapMarker(new PointLatLng(43.238949, 76.889709));
            marker.ZIndex = 10;
            MapControl.Markers.Add(marker);
        }
    }
}
