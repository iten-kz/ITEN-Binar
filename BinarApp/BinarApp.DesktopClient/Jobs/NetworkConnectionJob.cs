using BinarApp.DesktopClient.Units;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.Jobs
{
    public class NetworkConnectionJob : IJob
    {
        private NetworkUtils _networkUtils;

        public NetworkConnectionJob()
        {
            _networkUtils = new NetworkUtils();
        }

        public Task Execute(IJobExecutionContext context)
        {
            _networkUtils.CheckConnection();
            return Task.FromResult(0);
        }
    }
}
