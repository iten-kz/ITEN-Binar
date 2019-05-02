using BinarApp.Core.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Models
{
    public class FixationVM
    {
        public string Description { get; set; }
        public int Speed { get; set; }
        public DateTime FixationDate { get; set; }
        public string GRNZ { get; set; }
        public string IntruderFirstName { get; set; }
        public string IntruderLastName { get; set; }
        public string IntruderMiddleName { get; set; }
        public string IntruderIIN { get; set; }
        public DateTime IntruderBirthDate { get; set; }
        public string ImgBase64 { get; set; }
    }

    public class CameraFixationVM
    {
        public string PlateNumber { get; set; }
        public DateTime FixationDate { get; set; }
    }

    public class FixationsVM
    {
        public List<Fixation> Value { get; set; }
    }

    public class EquipmentsVM
    {
        public List<Equipment> Value { get; set; }
    }
}
