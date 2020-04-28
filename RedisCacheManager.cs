using System;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace licenseDemoNetCore
{
    public class RedisCacheManager
    {
        private RedisCacheOptions options;
        public RedisCacheManager()
        {
            options = new RedisCacheOptions
            {
                Configuration = "localhost",
                InstanceName = "SampleInstance"
            };
        }
        public string Get(string key)
        {
            using (var redisCache = new RedisCache(options))
            {
                var valueString = redisCache.GetString(key);
                if (!string.IsNullOrEmpty(valueString))
                {
                    return valueString;
                }

                return null;
            }
        }

        public void Set(string key, string token, int expiration)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiration)
            };

            using (var redisCache = new RedisCache(options))
            {
                redisCache.SetString(key, token, cacheOptions);
            }
        }

        public void Remove(string key)
        {
            using (var redisCache = new RedisCache(options))
            {
                redisCache.Remove(key);
            }
        }

        public int Count()
        {
            using (var redisConnection = ConnectionMultiplexer.Connect(options.Configuration))
            {
                var redisServer = redisConnection.GetServer(redisConnection.GetEndPoints().First());
                var redisDatabase = redisConnection.GetDatabase();
                return redisServer.Keys().Count();
            }
        }
    }
}