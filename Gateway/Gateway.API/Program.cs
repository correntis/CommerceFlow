using Gateway.Abstractions;
using Gateway.API.Abstractions;
using Gateway.API.Services;
using Gateway.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddOptions();
services.AddApplicationMetrics();

services.AddControllers();
services.AddProblemDetails();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddLogging(builder => builder.AddConsole());

services.AddGatewayCookieAuthentication(configuration);

services.AddScoped<IAuthService, AuthServiceClient>();
services.AddScoped<IUsersService ,UsersServiceClient>();
services.AddScoped<ITokenService , TokenService>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
