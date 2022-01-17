using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace RyderGyde.ShopNotes.RedisUpdateTrigger.Extensions
{
    internal static class RedisCacheExtensions
    {
        internal static async Task SetRecordAsync<T>(this IDistributedCache cache, string recordId, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            if (string.IsNullOrWhiteSpace(recordId)) return;

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromDays(7),
                SlidingExpiration = unusedExpireTime ?? TimeSpan.FromDays(2)
            };

            var jsonData = JsonConvert.SerializeObject(data);
            await cache.SetStringAsync(recordId, jsonData, options);
        }

        internal static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            if (string.IsNullOrWhiteSpace(recordId)) return default;

            var jsonData = await cache.GetStringAsync(recordId);

            return jsonData is null ? default : JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}