using System.Threading.Tasks;
using BinarApp.Utils;
using Quartz;

namespace BinarApp.Jobs
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
