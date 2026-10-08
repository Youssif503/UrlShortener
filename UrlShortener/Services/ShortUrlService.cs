using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.Common;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.Services.Abstraction;

namespace UrlShortener.Services;

public class ShortUrlService(
    ApplicationDbContext _context,
    IConfiguration _configuration,
    ICacheService _cacheService) : IShortUrlService
{
    public async Task<Result<Url>> AddShortUrlAsync(string requestUrl)
    {
        int attempts = 3;

        while (attempts > 0)
        {
            string shortCode = CreateShortCode();

            string shortUrl =
                $"{_configuration["Domain"]!}/{shortCode}";

            var newUrl = new Url
            {
                ShortUrl = shortUrl,
                LongUrl = requestUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _context.AddAsync(newUrl);

            try
            {
                await _context.SaveChangesAsync();

                return Result<Url>.Success(newUrl);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException postgresException &&
                    postgresException.SqlState == "23505")
                {
                    _context.Entry(newUrl).State = EntityState.Detached;

                    attempts--;
                    continue;
                }

                throw;
            }
        }

        return Result<Url>.Fail(
            ["Could not generate a unique short URL."]
        );
    }

    public async Task<Result<Url>> ResolveAsync(string shortCode)
    {
        // Same key for GET and SET
        var key = $"url:{shortCode}";

        // Try Redis first
        var cachedUrl = await _cacheService.GetAsync<Url>(key);

        if (cachedUrl is not null)
        {
            // Cache Hit
            return Result<Url>.Success(cachedUrl);
        }

        // Cache Miss → Database
        var url = await _context.Urls
            .FirstOrDefaultAsync(x =>
                x.ShortUrl.EndsWith($"/{shortCode}"));

        if (url is null)
        {
            return Result<Url>.Fail(
                ["The Url Does Not Found Yasta"]
            );
        }

        // Store in Redis
        var ttl = TimeSpan.FromDays(1);

        await _cacheService.SetAsync(
            key,
            url,
            ttl
        );

        return Result<Url>.Success(url);
    }

    private string CreateShortCode()
    {
        const string characters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        StringBuilder builder = new(8);

        for (int i = 0; i < 8; i++)
        {
            int randomNumber =
                RandomNumberGenerator.GetInt32(characters.Length);

            builder.Append(characters[randomNumber]);
        }

        return builder.ToString();
    }
}