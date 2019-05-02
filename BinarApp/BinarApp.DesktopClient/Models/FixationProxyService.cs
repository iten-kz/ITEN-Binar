using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using Newtonsoft.Json;
using NLog;
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
    public class FixationProxyService : IProxyService<Fixation>
    {
        private string _apiUrl;

        private HttpClient _httpClient;

        private EmployeePlateNumberService _srv;

        public FixationProxyService(EmployeePlateNumberService srv)
        {
            _apiUrl = ConfigurationManager.AppSettings["API_URL"].ToString();
            _httpClient = new HttpClient();
            _srv = srv;
        }

        public async Task<List<Fixation>> GetCollection(string filterQuery = "")
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = new List<Fixation>();

            var response = await _httpClient.GetAsync(_apiUrl + filterQuery);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<FixationsVM>(content);

                if (data != null)
                    result = data.Value;
            }

            return result;
        }

        public async Task PostEntity(Fixation entity)
        {
            // PPN Nickname
            entity.NickName = _srv.PlateNumber;

            var data = JsonConvert.SerializeObject(entity);

            var stringContent = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUrl + "/Fixations", stringContent);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.Content.ToString());
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
