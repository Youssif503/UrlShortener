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
    IConfiguration _configuration) : IShortUrlService
{
    public async Task<Result<Url>> AddShortUrlAsync(string requestUrl)
    {
        int attempts = 3;

        while (attempts > 0)
        {
            string shortCode = CreateShortCode();
            string shortUrl = $"{_configuration["domain"]!}/{shortCode}";

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

    public async Task<Result<Url>> ResolveAsync(string ShortUrl)
    {
        var url = await _context.Urls.FirstOrDefaultAsync(x=>x.ShortUrl == ShortUrl);

        if(url is null)
           return Result<Url>.Fail(["The Url Does Not Found Yasta"]);

        return Result<Url>.Success(url);
    }

    private string CreateShortCode()
    {
        const string digits =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        StringBuilder builder = new(8);

        for (int i = 0; i < 8; i++)
        {
            int randomNumber =
                RandomNumberGenerator.GetInt32(digits.Length);

            builder.Append(digits[randomNumber]);
        }

        return builder.ToString();
    }
}