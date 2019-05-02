using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.POCO
{
    public class FixationDetail
    {
        public int Id { get; set; }

        public int FixationId { get; set; }
        public virtual Fixation Fixation { get; set; }

        [MaxLength]
        public string Image { get; set; }

        public string ImagePlate { get; set; }

        public DateTime Date { get; set; }
    }
}
