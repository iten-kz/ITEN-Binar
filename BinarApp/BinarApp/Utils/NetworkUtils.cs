using BinarApp.Core.Interfaces;
using BinarApp.Core.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Utils
{
    public class NetworkUtils// : INetworkUtils
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public static event EventHandler<NetworkStatusEventArgs> NetworkStatusChanged;
        private static bool _networkConnectionIsAvailable = false;

        public static bool NetworkConnectionIsAvailable
        {
            get { return _networkConnectionIsAvailable; }
        }

        public NetworkUtils()
        {

        }

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        //Creating a function that uses the API function...
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        public void CheckConnection()
        {
            bool internetAvailable = IsConnectedToInternet();
            // Raise event
            _networkConnectionIsAvailable = internetAvailable;
            OnNetworkStatusChange(internetAvailable);
        }

        private async Task CheckConnection(string URL)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var url = ConfigurationManager.AppSettings["API_URL"].ToString();
                    await client.OpenReadTaskAsync(url); //("http://www.google.com/");
                    // Raise success event
                    _networkConnectionIsAvailable = true;
                    OnNetworkStatusChange(_networkConnectionIsAvailable);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Message: {ex.Message}, " +
                             $"stack trace: {ex.StackTrace}, " +
                             $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                // Raise failure event
                _networkConnectionIsAvailable = false;
                OnNetworkStatusChange(_networkConnectionIsAvailable);
            }
            finally
            {               
            }
        }

        // Events
        private void OnNetworkStatusChange(bool isAvailable)
        {
            NetworkStatusChanged?.Invoke(this, new NetworkStatusEventArgs
            {
                IsAvailable = isAvailable
            });
        }
    }
}
