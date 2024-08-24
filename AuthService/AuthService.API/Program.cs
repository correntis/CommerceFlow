using AuthService.Infrastructure.Configuration;
using AuthService.Infrastructure.Abstractions;
using AuthService.Infrastructure.Services;
using AuthService.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));


services.AddGrpc();
services.AddLogging(builder => { builder.AddConsole(); });

services.AddScoped<ICacheService, CacheService>();
services.AddScoped<ITokenService, TokenService>();

services.AddStackExchangeRedisCache(options =>
{
    var host = configuration["STORAGE_HOST"];
    var port = configuration["STORAGE_PORT"];
    var redisConfiguration = $"{host}:{port}";

    options.Configuration = redisConfiguration;
    options.InstanceName = "AuthService";
});

var app = builder.Build();

app.MapGrpcService<AuthServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
