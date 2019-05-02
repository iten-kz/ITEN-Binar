using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BinarApp.Core.Models
{
    public class ListViewEventArgs
    {
        public string PlateNumber { get; set; }
        public object Image { get; set; }
        public string Description { get; set; }
        public string WarningText { get; set; }
        public SolidColorBrush WarningColor { get; set; }
        public int Speed { get; set; }
        public DateTime FixationDate { get; set; }
    }
}
