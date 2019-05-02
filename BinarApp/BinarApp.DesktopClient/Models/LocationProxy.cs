using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.Models
{
    public class LocationProxy
    {
        public float Latitude { get; set; } = 15;

        public float Longitude { get; set; } = 17;

        public async Task<Location> GetLocation()
        {
            return new Location() { Latitude = Latitude, Longitude = Longitude };
        }
    }

    public class Location
    {
        public float Latitude { get; set; }

        public float Longitude { get; set; }
    }

}
