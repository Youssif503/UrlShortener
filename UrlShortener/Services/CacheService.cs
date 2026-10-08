
using System.Text.Json;
using StackExchange.Redis;
using UrlShortener.Models;
using UrlShortener.Services.Abstraction;

namespace UrlShortener.Services
{
    public class CacheService : ICacheService
    {
        private ILogger<CacheService> _logger;
        private IDatabase _redis;
        public CacheService(IConnectionMultiplexer multiplexer , ILogger<CacheService> logger)
        {
            _logger = logger;
            _redis = multiplexer.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _redis.StringGetAsync(key);

            if(value.IsNullOrEmpty){
            _logger.LogInformation("Cache Miss {key}",key);
            return default;
            }

            _logger.LogInformation("Cache Hit {key}",key);
            return JsonSerializer.Deserialize<T>(value.ToString());
        }

        public async Task RemoveAsync(string key)
        {
            var deleted = await _redis.KeyDeleteAsync(key);

            if (deleted)
                _logger.LogInformation("Cache Deleted {Key}", key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var SerilizedValue = JsonSerializer.Serialize(value);

            await _redis.StringSetAsync(key,
            SerilizedValue,
            expiration??TimeSpan.FromHours(3));

            _logger.LogInformation("Cache Setted {key}",key);
        }
    }
}