using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.Models
{
    public class LocationWorkerService : IDisposable
    {
        private LocationProxy _locProxy;

        public event EventHandler<LocationEventArg> LocationChanged;

        private bool _isContinue { get; set; }

        private bool _isStarted { get; set; } = false;

        private Task SearchAction;

        public List<List<Location>> PolygonCollection = new List<List<Location>>();

        public LocationWorkerService(LocationProxy locationProxy)
        {

            _locProxy = locationProxy;

            SearchAction = new Task(async () => 
            {
                if (_isContinue)
                {
                    var location = _locProxy.GetLocation();
                    
                }
            });
        }

        public void Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;
                SearchAction.Start();
            }

            _isContinue = true;
        }

        public void Stop()
        {
            _isContinue = false;
        }

        public void Dispose()
        {
            _isContinue = false;
        }
    }


    public class LocationEventArg : EventArgs
    {
        public LocationEnum LocationEnum { get; set; }

        public Location Location { get; set; }
    }

    public enum LocationEnum
    {
        Outside = 0,
        Inside = 1
    }

}
