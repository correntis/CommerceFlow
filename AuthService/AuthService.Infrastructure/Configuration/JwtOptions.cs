namespace AuthService.Infrastructure.Configuration
{
    public class JwtOptions
    {
        public string Key { get; init; }
        public string Issuer { get; init; }
        public string Audience { get; init; }
    }
}
