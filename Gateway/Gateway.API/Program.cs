using Gateway.Abstractions;
using Gateway.API.Abstractions;
using Gateway.API.Services;
using Gateway.Extensions;
using Gateway.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddOptions();

services.AddControllers();
services.AddProblemDetails();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddLogging(builder => builder.AddConsole());

services.AddGatewayCookieAuthentication(configuration);

services.AddScoped<IAuthService, AuthServiceClient>();
services.AddScoped<IUsersService ,UsersServiceClient>();


var app = builder.Build();

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
