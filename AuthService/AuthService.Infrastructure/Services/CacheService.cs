using AuthService.Infrastructure.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthService.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetTokenAsync(string token, string userId)
        {
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMonths(1));

            await _cache.SetStringAsync(token, userId.ToString(), options);
        }

        public async Task RemoveTokenAsync(string oldToken)
        {
            await _cache.RemoveAsync(oldToken);
        }

        public async Task<string> GetUserIdAsync(string token)
        {
            var userId = await _cache.GetStringAsync(token);

            return userId;
        }
    }
}
