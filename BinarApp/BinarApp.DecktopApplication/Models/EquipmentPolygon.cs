using BinarApp.Core.POCO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Models
{
    public class EquipmentPolygon
    {
        /* example location geoJSON
         * lat => y
         * lng => x
         * [
         *   {"lat":43.22300354422405,"lng":76.91076829194438},
         *   {"lat":43.224528042101255,"lng":76.91078974360553},
         *   {"lat":43.224528042101255,"lng":76.91078974360553},
         *   {"lat":43.224418592241896,"lng":76.90832280257143},
         *   {"lat":43.224418592241896,"lng":76.90832280257143},
         *   {"lat":43.22280027496003,"lng":76.90867675498068},
         *   {"lat":43.22280027496003,"lng":76.90867675498068}]
        */

        public Equipment Equipment { get; private set; }

        public bool? HasPolygon { get; set; }

        public EquipmentLocationModel BottomLeft { get; private set; }
        
        public EquipmentLocationModel TopRight { get; private set; }
        
        public EquipmentPolygon(Equipment equipment)
        {
            Equipment = equipment;

            if (!string.IsNullOrEmpty(equipment.GeoJson))
            {
                var data = JsonConvert.DeserializeObject<List<EquipmentLocationModel>>(equipment.GeoJson);

                var lngs = data.Select(x => x.Lng).OrderBy(x => x).ToList();
                var lats = data.Select(x => x.Lat).OrderBy(x => x).ToList();

                BottomLeft = new EquipmentLocationModel() { Lat = lats.First(), Lng = lngs.First() };
                TopRight = new EquipmentLocationModel() { Lat = lats.Last(), Lng = lngs.Last() };
            }
        }
    }
}
