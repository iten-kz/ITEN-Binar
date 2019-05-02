using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Models
{
    public class TcpListenerEventArgs : EventArgs
    {
        public string Data { get; set; }
    }
}
