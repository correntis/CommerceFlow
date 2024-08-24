namespace AuthService.Infrastructure.Configuration
{
    public record JwtOptions(
        string Key,
        string Issuer,
        string Audience
    );
}
