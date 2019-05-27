using BinarApp.Core.POCO;
using BinarApp.DecktopApplication.Proxies;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Models
{
    public class LocationManager : IDisposable
    {
        private GeoCoordinateManager _geoCoordinateManager;

        private ProxyService<Equipment> _equipmentProxyService;

        private List<EquipmentPolygon> _equipmentPolygons;

        public event EventHandler<LocationEventArg> LocationChanged;

        public LocationManager(GeoCoordinateManager geoCoordinateManager, ProxyService<Equipment> equipmentProxyService)
        {
            _geoCoordinateManager = geoCoordinateManager;
            _equipmentProxyService = equipmentProxyService;

            _geoCoordinateManager.GeoCoordinateChanged += _geoCoordinateManager_GeoCoordinateChanged;
            _geoCoordinateManager.GeoWatcherStatusChanged += _geoCoordinateManager_GeoWatcherStatusChanged;
        }

        private void _geoCoordinateManager_GeoWatcherStatusChanged(object sender, bool e)
        {
            var arg = new LocationEventArg();
            LocationChanged(this, arg);
        }

        private void _geoCoordinateManager_GeoCoordinateChanged(object sender, GeoCoordinate e)
        {
            if (_equipmentPolygons == null)
                return;

            var insidePolygons = _equipmentPolygons.Where(x =>
                (e.Latitude >= x.BottomLeft.Lat && e.Longitude >= x.BottomLeft.Lng)
                && (e.Latitude <= x.TopRight.Lat && e.Longitude <= x.TopRight.Lng))
                .Select(x => new
                {
                    x,
                    DiffLat = x.TopRight.Lat - x.BottomLeft.Lat,
                    DiffLng = x.TopRight.Lng - x.BottomLeft.Lng
                });

            var currentPolygon = insidePolygons
                .Select(x => new
                {
                    x.x,
                    Range = Math.Sqrt(Math.Pow(x.DiffLat - e.Latitude, 2) + Math.Pow(x.DiffLng - e.Longitude, 2))
                })
                .OrderBy(x => x.Range)
                .Select(x => x.x)
                .FirstOrDefault();

            //var currentPolygon = _equipmentPolygons.FirstOrDefault(x =>
            //    (e.Latitude >= x.BottomLeft.Lat && e.Longitude >= x.BottomLeft.Lng)
            //    && (e.Latitude <= x.TopRight.Lat && e.Longitude <= x.TopRight.Lng));

            var arg = new LocationEventArg() {GeoCoordinate = e };

            if (currentPolygon != null)
            {
                arg.CurrentEquipment = currentPolygon.Equipment;
                arg.IsInEquipment = true;
            }

            if (LocationChanged != null)
                LocationChanged(this, arg);
        }

        public async Task Start()
        {
            var equipments = await GetEquipments();
            _equipmentPolygons = equipments.Select(x => new EquipmentPolygon(x)).ToList();
        }

        private async Task<ICollection<Equipment>> GetEquipments()
        {
            var res = await _equipmentProxyService.GetCollection("Equipments");
            return res;
        }
        
        public void Dispose()
        {
            if (_geoCoordinateManager != null)
            {
                _geoCoordinateManager.GeoCoordinateChanged -= _geoCoordinateManager_GeoCoordinateChanged;
                _geoCoordinateManager.GeoWatcherStatusChanged -= _geoCoordinateManager_GeoWatcherStatusChanged;
                _geoCoordinateManager.Dispose();
                _geoCoordinateManager = null;
            }

            if (_equipmentProxyService != null)
            {
                _equipmentProxyService.Dispose();
                _equipmentProxyService = null;
            }

        }
    }
}
