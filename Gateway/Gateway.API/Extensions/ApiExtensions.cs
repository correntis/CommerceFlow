using Gateway.Abstractions;
using Gateway.Infrastructure;
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
            var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
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
                                    var authClient = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                                    var verifyResponse = await authClient.VerifyAsync(refreshToken);

                                    if (verifyResponse.IsValid)
                                    {
                                        context.Response.Cookies.Append("accessToken", verifyResponse.AccessToken,
                                            new CookieOptions() { HttpOnly = true, Expires = DateTime.UtcNow.AddMonths(1) }
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
