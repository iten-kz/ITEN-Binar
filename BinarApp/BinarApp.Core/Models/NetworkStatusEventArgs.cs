using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Models
{
    public class NetworkStatusEventArgs : EventArgs
    {
        public bool IsAvailable { get; set; }
    }
}
