using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Services.Abstraction
{
    public interface ICacheService
    {
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task<T?> GetAsync<T>(string key);
    Task RemoveAsync(string key);

    }
}