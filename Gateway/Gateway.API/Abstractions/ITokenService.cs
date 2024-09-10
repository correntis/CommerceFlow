using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Gateway.API.Abstractions
{
    public interface ITokenService
    {
        Task<string> HandleUpdateTokenAsync(HttpContext context);
    }
}