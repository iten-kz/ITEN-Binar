using BinarApp.Core.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Models
{
    public class EquipmentMovement
    {
        public Equipment Equipment { get; set; }

        public DateTime Start { get; set; }

        public DateTime? Finish { get; set; }

        public bool IsSended { get; set; }
    }
}
