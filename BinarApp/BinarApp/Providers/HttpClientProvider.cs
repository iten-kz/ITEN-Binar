using BinarApp.Core.Interfaces;
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

namespace BinarApp.Providers
{
    public class HttpClientProvider : IHttpClientProvider
    {
        public string ApiURL;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public HttpClientProvider()
        {
            ApiURL = ConfigurationManager.AppSettings["API_URL"].ToString();
        }

        public async Task<List<Fixation>> GetFixationsAsync(string queryString = "")
        {
            var httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri(ApiURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            List<Fixation> data = new List<Fixation>();
            string queryOptions = queryString;

            HttpResponseMessage response = await httpClient.GetAsync(ApiURL + queryOptions);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<List<Fixation>>(content);
            }            
            return data;
        }

        public async Task<bool> SendFixationAsync(Fixation fixation)
        {
            var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Accept.Clear();
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var data = JsonConvert.SerializeObject(fixation);
            var buffer = Encoding.UTF8.GetBytes(data);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = null;
            bool result = false;
            try
            {
                response = await httpClient.PostAsync(ApiURL + "/Fixations", byteContent);
                result = response != null ? response.IsSuccessStatusCode : false;
            }
            catch (Exception ex)
            {
                _logger.Error($"Message: {ex.Message}, " +
                   $"stack trace: {ex.StackTrace}, " +
                   $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                result = false;
            }
         
            //response.EnsureSuccessStatusCode();
            // return URI of the created resource.
            return result;
        }

        public async Task<List<Intruder>> GetIntrudersAsync(string queryString = "")
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            List<Intruder> data = new List<Intruder>();            
            //var str = "$filter = IIN eq 'Toys'";
            HttpResponseMessage response = await httpClient.GetAsync(ApiURL+ "/Intruders?" + queryString);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<List<Intruder>>(content);
            }

            return data;
        }

    }
}
