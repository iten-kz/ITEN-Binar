using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.Models
{
    public class EquipmentProxyService : IProxyService<Equipment>
    {
        private string _apiUrl;

        private HttpClient _httpClient;

        public EquipmentProxyService()
        {
            _apiUrl = ConfigurationManager.AppSettings["API_URL"].ToString();
            _httpClient = new HttpClient();
        }

        public async Task<List<Equipment>> GetCollection(string filterQuery = "")
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = new List<Equipment>();

            var response = await _httpClient.GetAsync(_apiUrl + filterQuery);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<EquipmentsVM>(content);

                if (data != null)
                    result = data.Value;
            }

            return result;
        }

        public Task PostEntity(Equipment entity)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
                _httpClient = null;
            }
        }
    }
}
