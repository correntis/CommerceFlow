using AuthService.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddGrpc();
services.AddLogging(builder => { builder.AddConsole(); });

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
