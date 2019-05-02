using BinarApp.Core.Models;
using System;

namespace BinarApp.Core.Interfaces
{
    public interface IFolderListenerProvider
    {
        void StartListener();
        void StopListener();
        event EventHandler<FileReceivedEventArgs> FileReceived;
    }
}
