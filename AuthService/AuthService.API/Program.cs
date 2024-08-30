using AuthService.Infrastructure.Configuration;
using AuthService.Infrastructure.Abstractions;
using AuthService.Infrastructure.Services;
using AuthService.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

Console.WriteLine(builder.Environment.EnvironmentName);

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
services.Configure<EmailOptions>(configuration.GetSection(nameof(EmailOptions)));

services.AddGrpc();
services.AddLogging(builder => { builder.AddConsole(); });

services.AddScoped<ITokenCacheService, TokenCacheService>();
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<IEmailService, EmailService>();

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
