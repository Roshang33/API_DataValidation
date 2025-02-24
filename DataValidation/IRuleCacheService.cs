using System.Data;

namespace DataValidation
{
    public interface IRuleCacheService
    {
        Rule? GetCachedRule(string ruleName);
        void SetCachedRule(string ruleName, Rule rule);
        void RemoveCachedRule(string ruleName);
    }
}

