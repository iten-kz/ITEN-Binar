using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Proxies
{
    public class ProxyService<TEntity>: IDisposable 
        where TEntity: class
    {
        private string _apiUrl;

        private HttpClient _httpClient;

        private string _entityName;

        public ProxyService()
        {
            _apiUrl = ConfigurationManager.AppSettings["API_URL"].ToString();
            _httpClient = new HttpClient();
        }

        public async Task<List<TEntity>> GetCollection(string entityName,string filterQuery = "")
        {
            _entityName = entityName;

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = new List<TEntity>();

            var query = string.Format("{0}/{1}{2}",
                _apiUrl,
                _entityName,
                 filterQuery);

            try
            {
                var response = await _httpClient.GetAsync(query);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<ProxyResult<TEntity>>(content);

                    if (data != null)
                        result = data.Value;
                }
            }
            catch (Exception ex)
            {

            }

            

            return result;
        }

        public async Task Post(TEntity entity)
        {
            var data = JsonConvert.SerializeObject(entity);

            var query = string.Format("{0}/{1}",
                _apiUrl,
                _entityName);

            var stringContent = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(query, stringContent);

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
