using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UrlShortener.Data;
using UrlShortener.Services;
using UrlShortener.Services.Abstraction;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(options =>
{
    var config = builder.Configuration["Redis:ConnectionString"];
    return ConnectionMultiplexer.Connect(config!);
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));
    
builder.Services.AddScoped<ICacheService,CacheService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();