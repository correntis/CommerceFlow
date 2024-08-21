using AuthService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    var host = builder.Configuration["STORAGE_HOST"];
    var port = builder.Configuration["STORAGE_PORT"];
    var configuration = $"{host}:{port}";

    options.Configuration = configuration;
    options.InstanceName = "AuthService";
});

var app = builder.Build();

app.MapGrpcService<AuthServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
