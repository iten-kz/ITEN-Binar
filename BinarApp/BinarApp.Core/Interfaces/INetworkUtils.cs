using BinarApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Interfaces
{
    public interface INetworkUtils
    {
        void CheckConnection();
        bool NetworkConnectionIsAvailable { get; }
        event EventHandler<NetworkStatusEventArgs> NetworkStatusChanged;
    }
}
