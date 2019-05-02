using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Models
{
    public class FixationIncidentViewModel
    {
        public string PlateNumber { get; set; }

        public int EquipmentId { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateFinish { get; set; }

        public string FirstImage { get; set; }

        public string FirstImagePlate { get; set; }

        public string LastImage { get; set; }

        public string LastImagePlate { get; set; }
    }
}
