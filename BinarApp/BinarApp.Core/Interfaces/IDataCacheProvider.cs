using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using System.Collections.Generic;

namespace BinarApp.Core.Interfaces
{
    public enum CacheDataType
    {
        DatabaseCache,
        CameraPlateCache,
        BinarCache
    };

    public interface IDataCacheProvider
    {
        void SaveDataToFile<T>(List<T> items, CacheDataType type);

        void CleanCache(CacheDataType type);

        List<T> GetDataFromCache<T>(CacheDataType type);

        void RemoveItemFromCache<T>(CacheDataType type, T item);
    }
}
