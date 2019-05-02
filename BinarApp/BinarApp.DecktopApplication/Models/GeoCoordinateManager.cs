using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;

namespace BinarApp.DecktopApplication.Models
{
    public class GeoCoordinateManager : IDisposable
    {
        private GeoCoordinateWatcher _watcher = null;

        public event EventHandler<bool> GeoWatcherStatusChanged;

        public event EventHandler<GeoCoordinate> GeoCoordinateChanged;

        public GeoCoordinateManager()
        {
            _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            _watcher.StatusChanged += _watcher_StatusChanged;
            _watcher.PositionChanged += _watcher_PositionChanged;
            _watcher.Start();
        }

        private void _watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            var status = e.Status == GeoPositionStatus.Ready;

            if (GeoWatcherStatusChanged != null)
                GeoWatcherStatusChanged(this, status);
        }

        private void _watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // TODO
            if (GeoCoordinateChanged != null)
                GeoCoordinateChanged(this, e.Position.Location);
        }

        public void Dispose()
        {
            _watcher.StatusChanged -= _watcher_StatusChanged;
            _watcher.PositionChanged -= _watcher_PositionChanged;
            _watcher.Stop();
            _watcher.Dispose();
            _watcher = null;
        }
    }
}
