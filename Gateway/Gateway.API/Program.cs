using Gateway;
using Gateway.Abstractions;
using Gateway.Extensions;
using Gateway.Infrastructure;
using Gateway.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddLogging(builder => builder.AddConsole());

services.AddGatewayCookieAuthentication(configuration);

services.AddSingleton<IAuthService, AuthServiceClient>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
