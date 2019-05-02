using BinarApp.Core.POCO;
using BinarApp.Providers;
using BinarApp.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Jobs
{
    public class DataCacheJob : IJob
    {
        private DataCacheProvider _dataCacheProvider;
        private HttpClientProvider _httpClientProvider;        

        public DataCacheJob()
        {
            _dataCacheProvider = new DataCacheProvider();
            _httpClientProvider = new HttpClientProvider();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var fixations = _dataCacheProvider.GetDataFromCache<Fixation>(Core.Interfaces.CacheDataType.DatabaseCache);
            if (fixations == null || fixations.Count == 0)
            {
                if (NetworkUtils.NetworkConnectionIsAvailable)
                {
                    //string query = $"/Fixations?$orderby=FixationDate desc";
                    string query = $"/Fixations?$filter=Id gt 298494&$orderby=FixationDate desc";
                    fixations = await _httpClientProvider.GetFixationsAsync(query);
                    _dataCacheProvider.SaveDataToFile<Fixation>(fixations, Core.Interfaces.CacheDataType.DatabaseCache);
                }
            }
            else
            {                
                int lastId = fixations.Max(x => x.Id);
                string query = $"/Fixations?$filter=Id gt {lastId}&$orderby=FixationDate desc";
                fixations = await _httpClientProvider.GetFixationsAsync(query);
                _dataCacheProvider.SaveDataToFile<Fixation>(fixations, Core.Interfaces.CacheDataType.DatabaseCache);
            }
        }
    }
}
