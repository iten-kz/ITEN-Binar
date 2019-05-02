using BinarApp.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.Providers
{
    public class DataCacheProvider : IDataCacheProvider
    {
        private List<string> _filePaths;
        private string _baseDir = string.Empty;

        public DataCacheProvider()
        {
            _filePaths = new List<string>();
            _baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _filePaths.Add($"{_baseDir}\\cache\\FixationDBCache.json");
            _filePaths.Add($"{_baseDir}\\cache\\CameraFixationsCache.json");
            _filePaths.Add($"{_baseDir}\\cache\\BinarCache.json");

            if (!Directory.Exists($"{_baseDir}\\cache"))
            {
                Directory.CreateDirectory($"{_baseDir}\\cache");
            }

            foreach (var item in _filePaths)
            {
                if (!File.Exists(item))
                {
                    File.Create(item);
                }
            }
        }

        public void SaveDataToFile<T>(List<T> items, CacheDataType type)
        {
            string cacheName = this.GetCacheNameByType(type);
            string filePath = $"{_baseDir}\\cache\\{cacheName}.json";

            var data = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath)) ?? new List<T>();

            data.AddRange(items);
            string str = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, str);
        }

        public void RemoveItemFromCache<T>(CacheDataType type, T item)
        {
            string cacheName = this.GetCacheNameByType(type);
            string filePath = $"{_baseDir}\\cache\\{cacheName}.json";

            var data = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath)) ?? new List<T>();
            data.Remove(item);

            string str = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, str);
        }

        public void CleanCache(CacheDataType type)
        {
            string cacheName = this.GetCacheNameByType(type);
            string filePath = $"{_baseDir}\\cache\\{cacheName}.json";
            File.WriteAllText(filePath, string.Empty);
        }

        public List<T> GetDataFromCache<T>(CacheDataType type)
        {
            string cacheName = string.Empty;
            DateTimeOffset expiration = DateTimeOffset.MaxValue;
            switch (type)
            {
                case CacheDataType.DatabaseCache:
                    cacheName = "FixationDBCache";
                    expiration = DateTimeOffset.MaxValue;
                    break;
                case CacheDataType.CameraPlateCache:
                    cacheName = "CameraFixationsCache";
                    expiration = DateTimeOffset.Now.AddHours(24.0);
                    break;
                case CacheDataType.BinarCache:
                    cacheName = "BinarCache";
                    expiration = DateTimeOffset.Now.AddDays(30);
                    break;
            }

            ObjectCache cache = MemoryCache.Default;
            string fileContents = cache[cacheName] as string;
            if (fileContents == null)
            {
                // Fetch the file contents.
                string filePath = $"{_baseDir}\\cache\\{cacheName}.json";
                fileContents = File.ReadAllText(filePath);

                var policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = expiration;
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(_filePaths.Where(x => x == filePath).ToList()));

                cache.Set(cacheName, fileContents, policy);
            }

            return JsonConvert.DeserializeObject<List<T>>(fileContents);
        }

        private string GetCacheNameByType(CacheDataType type)
        {
            string cacheName = string.Empty;
            switch (type)
            {
                case CacheDataType.DatabaseCache:
                    cacheName = "FixationDBCache";
                    break;
                case CacheDataType.CameraPlateCache:
                    cacheName = "CameraFixationsCache";
                    break;
                case CacheDataType.BinarCache:
                    cacheName = "BinarCache";
                    break;
            }

            return cacheName;
        }
    }
}
