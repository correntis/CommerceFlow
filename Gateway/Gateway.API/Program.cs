using Elastic.Clients.Elasticsearch;
using Gateway.Abstractions;
using Gateway.API.Abstractions;
using Gateway.API.Services;
using Gateway.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddOptions();
services.AddApplicationMetrics();

services.AddControllers();
services.AddProblemDetails();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddSerilog();
});

services.AddGatewayCookieAuthentication(configuration);
services.AddElastic(builder, configuration);

services.AddScoped<IAuthService, AuthServiceClient>();
services.AddScoped<IUsersService ,UsersServiceClient>();
services.AddScoped<IProductsService, ProductsServiceClient>();
services.AddScoped<ITokenService , TokenService>();


var app = builder.Build();

app.UseCors(policy =>
{
    policy.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
