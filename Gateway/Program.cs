using Gateway;
using Gateway.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.HttpContext.Request.Cookies.TryGetValue("accessToken", out var accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = async context =>
            {
        
                if (context.Request.Cookies.ContainsKey("refreshToken"))
                {
                    var refreshToken = context.Request.Cookies["refreshToken"];
                    var authClient = context.HttpContext.RequestServices.GetRequiredService<AuthServiceClient>();
                    var verifyResponse = await authClient.VerifyAsync(refreshToken);

                    if (verifyResponse.IsValid)
                    {
                        context.Response.Cookies.Append("accessToken", verifyResponse.AccessToken,
                            new CookieOptions()
                            {
                                HttpOnly = true,
                                Expires = DateTime.UtcNow.AddDays(3)
                            }
                        );

                        context.Response.Cookies.Append("refreshToken", verifyResponse.AccessToken,
                            new CookieOptions()
                            {
                                HttpOnly = true,
                                Expires = DateTime.UtcNow.AddMonths(1)
                            }
                        );

                        await context.HttpContext.AuthenticateAsync();
                        
                        context.Response.StatusCode = 200;

                        return;
                    }
                }
                

                context.Response.Headers.Append("TokenExpired", "true");
            }
        };
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
