using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShortener.Common;
using UrlShortener.Models;

namespace UrlShortener.Services.Abstraction
{
    public interface IShortUrlService
    {
        public Task<Result<Url>> AddShortUrlAsync(string LongUrl);
        public Task<Result<Url>> ResolveAsync(string ShortUrl);
    }
}