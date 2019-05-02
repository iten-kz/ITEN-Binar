using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.POCO
{
    public class Equipment
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string GeoJson { get; set; }

        public string DayImage { get; set; }

        public string NightImage { get; set; }

        public virtual ICollection<Fixation> Fixations { get; set; }
    }
}
