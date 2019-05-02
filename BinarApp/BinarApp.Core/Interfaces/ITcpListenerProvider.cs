using BinarApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Interfaces
{
    public interface ITcpListenerProvider
    {
        void StartListener();
        void StopListener();
        event EventHandler<TcpListenerEventArgs> DataReceived;
    }
}
