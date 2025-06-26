using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace CGHSBilling.Caching
{
    public static class MemoryCaching
    {
        public static bool CacheKeyExist(string key)
        {
            MemoryCache cache = MemoryCache.Default;
            return cache.Contains(key);
        }
        public static object GetCacheValue(string key)
        {
            MemoryCache cache = MemoryCache.Default;
            if (cache.Contains(key))
                return cache.Get(key);
            else
                return null;
        }

        public static void AddCacheValue(string key, object value)
        {
            MemoryCache cache = MemoryCache.Default;
            cache.Add(key, value, DateTimeOffset.UtcNow.AddHours(12));
        }

        public static object GetOrAddCacheValue(string key, object value)
        {
            MemoryCache cache = MemoryCache.Default;
            if (!CacheKeyExist(key))
            {
                cache.Add(key, value, DateTimeOffset.UtcNow.AddHours(12));
            }

            return cache.Get(key);
        }

        public static void RemoveCacheValue(string key)
        {
            MemoryCache cache = MemoryCache.Default;
            if (CacheKeyExist(key))
                cache.Remove(key);
        }

        public static void ClearAllCache()
        {
            MemoryCache cache = MemoryCache.Default;
            List<string> cacheKeys = cache.Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                cache.Remove(cacheKey);
            }
        }


    }

    enum CachingKeys
    {
        ServiceTypeMaster = 0,
        ServiceMaster = 1,
        State = 2,
        City = 3,
        PatientType = 4,
        RoomType = 5,
        AdmissionType = 6,
        ManagementType = 7,
        GetAllClient =8,
        Notification = 9,
        ActiveGender = 10,
        SurgeryMaster = 11,
        DefaultServiceMasters = 12,
        AllBedcharges=13,
        LinkedServices = 14,
        DefaultServiceMasters_Rates = 15,
        OPDServices = 16,
        CancerSurgery=17,
        ServiceCategory=18,
        ManagementLinking =19,
        HopePatients = 20,
        ServicesList = 21
    }
}