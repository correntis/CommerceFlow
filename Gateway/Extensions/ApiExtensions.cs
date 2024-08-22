using Gateway.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Gateway.Extensions
{
    public static class ApiExtensions
    {
        public static void AddGatewayCookieAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
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
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                if (context.Request.Cookies.ContainsKey("refreshToken"))
                                {
                                    var refreshToken = context.Request.Cookies["refreshToken"];
                                    var authClient = context.HttpContext.RequestServices.GetRequiredService<AuthServiceClient>();
                                    var verifyResponse = await authClient.VerifyAsync(refreshToken);

                                    if (verifyResponse.IsValid)
                                    {
                                        context.Response.Cookies.Append("accessToken", verifyResponse.AccessToken,
                                            new CookieOptions() { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(3) }
                                        );

                                        context.Response.Cookies.Append("refreshToken", verifyResponse.RefreshToken,
                                            new CookieOptions() { HttpOnly = true, Expires = DateTime.UtcNow.AddMonths(1) }
                                        );

                                        context.Response.StatusCode = StatusCodes.Status426UpgradeRequired;

                                        return;
                                    }
                                }
                            }
                        }
                    };
                });

            services.AddAuthorization();
        }
    }
}
