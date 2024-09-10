using Gateway.API.Abstractions;
using Gateway.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using System.Reflection;
using System.Text;
using Serilog.Sinks.Elasticsearch;
using System.IdentityModel.Tokens.Jwt;


namespace Gateway.Extensions
{
    public static class ApiExtensions
    {
        public static void AddOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        }

        public static void AddElastic(
            this IServiceCollection services,
            WebApplicationBuilder builder,
            IConfiguration configuration
            )
        {
            var environment = builder.Environment.EnvironmentName;

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console()
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configuration)
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticOptions:Url"]))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat =
                       $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}" +
                       $"-{environment?.ToLower().Replace(".", "-")}" +
                       $"-{DateTime.UtcNow:yyyy-MM}"
                })
                .CreateLogger();
        }

        public static void AddApplicationMetrics(
            this IServiceCollection services
            )
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resourse => resourse.AddService("Commerce Flow"))
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddPrometheusExporter();
                });
        }

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
                            if(context.Request.Cookies.TryGetValue("accessToken", out var accessToken))
                            {
                                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

                                if(jwtToken.ValidTo < DateTime.UtcNow)
                                {
                                    var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                                    var newAccessToken = tokenService.HandleUpdateTokenAsync(context.HttpContext).Result;

                                    if (!string.IsNullOrEmpty(newAccessToken))
                                    {
                                        accessToken = newAccessToken;   
                                    }
                                }

                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();
        }
    }
}
