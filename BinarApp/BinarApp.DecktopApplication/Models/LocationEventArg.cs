using BinarApp.Core.POCO;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Models
{
    public class LocationEventArg : EventArgs
    {
        public Equipment CurrentEquipment { get; set; }

        public GeoCoordinate GeoCoordinate { get; set; }

        public bool IsInEquipment { get; set; }
    }
}
