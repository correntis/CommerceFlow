using Microsoft.EntityFrameworkCore;
using CommerceFlow.Persistence;
using CommerceFlow.Persistence.Repositories;
using CommerceFlow.Persistence.Abstractions;
using UsersService.API.Abstractions;
using UsersService.API.Infrastructure;
using UsersService.API.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddGrpc();
services.AddDbContext<CommerceDbContext>(options =>
{
    options.UseNpgsql(
        $"User ID={configuration["POSTGRES_USER"]};" +
        $"Password={configuration["POSTGRES_PASSWORD"]};" +
        $"Host={configuration["POSTGRES_HOST"]};" +
        $"Port={configuration["POSTGRES_PORT"]};" +
        $"Database={configuration["POSTGRES_DB"]}"
    );       
});

services.AddScoped<IUsersRepository, UsersRepository>();
services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

app.MapGrpcService<UsersServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
