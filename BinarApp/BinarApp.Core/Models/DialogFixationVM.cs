using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BinarApp.Core.Models
{
    public class DialogFixationVM
    {
        public string FullName { get; set; }
        public string GRNZ { get; set; }
        public string FixationDate { get; set; }
        public string Description { get; set; }
        public int Speed { get; set; }
        public BitmapImage Image { get; set; }
        public string Place { get; set; }
    }
}
