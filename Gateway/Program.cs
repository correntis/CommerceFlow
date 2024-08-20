using Gateway;
using Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(builder => builder.AddConsole());

var authServiceHost = builder.Configuration["AUTH_HOST"];
var authServicePort = builder.Configuration["AUTH_PORT"];
builder.Services.AddSingleton(sp =>
{
    var logger = sp.GetRequiredService<ILogger<AuthServiceClient>>();

    var address = $"http://{authServiceHost}:{authServicePort}"; 

    return new AuthServiceClient(logger,address);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
