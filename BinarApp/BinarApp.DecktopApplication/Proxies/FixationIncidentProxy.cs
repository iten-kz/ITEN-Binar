using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Proxies
{
    public class FixationIncidentProxy : IDisposable
    {
        private string _apiUrl;

        private HttpClient _httpClient;

        public FixationIncidentProxy()
        {
            _apiUrl = ConfigurationManager.AppSettings["API_URL_2"].ToString();
            _httpClient = new HttpClient();
        }

        public async Task Post(ICollection<FixationIncidentViewModel> collection)
        {
            var enumCollection = collection.Select((x, i) => new { x, Index = Convert.ToInt32(i / 5) })
                .GroupBy(x => x.Index);

            foreach (var collectionItem in enumCollection)
            {
                var sendData = collectionItem.Select(x => x.x).ToList();
                var data = JsonConvert.SerializeObject(sendData);

                var query = string.Format("{0}/{1}",
                    _apiUrl,
                    "FixationIncident");

                var stringContent = new StringContent(data, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(query, stringContent);

                //if (!response.IsSuccessStatusCode)
                //    throw new HttpRequestException(response.Content.ToString());
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
