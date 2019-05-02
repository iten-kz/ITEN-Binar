using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BinarApp.API.Models
{
    [Serializable]
    public class FixationSerialiazibleModel
    {
        public DateTime FixationDate { get; set; }

        public string GRNZ { get; set; }
        
        public int? EquipmentId { get; set; }
        
        public List<FixationDetailSerialiazibleModel> FixationDetails { get; set; }
    }

    [Serializable]
    public class FixationDetailSerialiazibleModel
    {
        public string Image { get; set; }

        public string ImagePlate { get; set; }

        public DateTime Date { get; set; }
    }

}