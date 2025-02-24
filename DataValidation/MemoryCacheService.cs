using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace DataValidation
{
    public class RuleCacheService : IRuleCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10); // Cache expiry time

        public RuleCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Rule? GetCachedRule(string ruleName)
        {
            _memoryCache.TryGetValue(ruleName, out Rule? cachedRule);
            return cachedRule;
        }

        public void SetCachedRule(string ruleName, Rule rule)
        {
            _memoryCache.Set(ruleName, rule, _cacheExpiration);
        }

        public void RemoveCachedRule(string ruleName)
        {
            _memoryCache.Remove(ruleName);
        }
    }
}
