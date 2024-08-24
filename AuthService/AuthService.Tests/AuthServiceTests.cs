using AuthService.Infrascructure;
using AuthService.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AuthService.Tests
{
    public class AuthServiceTests
    {
        private readonly AuthServiceImpl _authService;
        private readonly IConfiguration _configuration;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<ILogger<AuthServiceImpl>> _mockLogger;


        public AuthServiceTests()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.Configure<JwtOptions>(_configuration.GetSection("JwtOptions"));

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>();

            _mockCache = new Mock<IDistributedCache>();
            _mockLogger = new Mock<ILogger<AuthServiceImpl>>();
            _authService = new AuthServiceImpl(_mockLogger.Object, _mockCache.Object, _jwtOptions);
        }

        [Fact]
        public void IssueAccessToken_ShouldReturnValidToken()
        {
            var userId = 123ul;

            var token = _authService.IssueAccessToken(userId);
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Assert.NotNull(token);
            Assert.Equal(_jwtOptions.Value.Issuer, jwtToken.Issuer);
            Assert.Equal(_jwtOptions.Value.Audience, jwtToken.Audiences.ToList()[0]);
            Assert.Equal(userId.ToString(), jwtToken.Claims.First(c => c.Type == "sub").Value);
            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public async void IssueRefreshToken_ShouldReturnRerfreshToken()
        {
            var userId = 123ul;

            var token = await _authService.IssueRefreshTokenAsync(userId);

            Assert.NotNull(token);
        }

        [Fact]
        public async void IssueRefreshToken_ShouldSaveTokenToCache()
        {
            var userId = 123ul;

            var token = await _authService.IssueRefreshTokenAsync(userId);

            Assert.NotEmpty(token);

            _mockCache.Verify(c => c.SetAsync(
                It.Is<string>(key => key == token),
                It.Is<byte[]>(value => Encoding.UTF8.GetString(value) == userId.ToString()),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async void RemoveRefreshTokenFromCache_ShouldRemoveToken()
        {
            var token = Guid.NewGuid().ToString();

            await _authService.RemoveRefreshTokenFromCacheAsync(token);

            _mockCache.Verify(c => c.RemoveAsync(
                It.Is<string>(key => key == token),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }
    }
}