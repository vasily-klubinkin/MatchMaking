using MatchMaking.Service.Application.POCO;
using MatchMaking.Service.Application.Repositories;
using StackExchange.Redis;

namespace MatchMaking.Service.Infra.Repositories;

// We use a global TTL to automatically clean up old match data
// instead of implementing reference counting or manual cleanup logic.
// This helps prevent Redis from growing indefinitely in memory.
//
// TTL = 30 days (can be tuned depending on business requirements).
// One match entry (formed_match + user_match mappings) ≈ 112 bytes.
//
// Estimated storage usage:
//  - 1 match/sec  ≈ 0.01 GB/month, 0.12 GB/year
//  - 10 matches/sec ≈ 0.1 GB/month, 1.2 GB/year
//  - 100 matches/sec ≈ 1 GB/month, 12 GB/year
//
// So with a 30-day TTL and 100 matches/sec, Redis will store ≈ 1 GB of data,
// which is safe and manageable for modern deployments.
internal class FormedMatchesRedisRepository : IFormedMatchesRepository
{
    private readonly IDatabase _db;
    private static readonly TimeSpan _ttl = TimeSpan.FromDays(30);
    
    private const string FORMED_MATCHES_KEY = "formed_matches";
    private const string USER_MATCHES_KEY = "user_matches";
    
    
    public FormedMatchesRedisRepository(IConnectionMultiplexer multiplexer)
    {
        _db = multiplexer.GetDatabase();
    }

    public async Task AddMatch(FormedMatch formedMatch, CancellationToken cancellationToken)
    {
        var matchKey = $"{FORMED_MATCHES_KEY}:{formedMatch.MatchId}";
        var userIds = formedMatch.UserIds.Select(u => (RedisValue)u).ToArray();

        cancellationToken.ThrowIfCancellationRequested();
        await _db.SetAddAsync(matchKey, userIds);
        
        await _db.KeyExpireAsync(matchKey, _ttl);
        cancellationToken.ThrowIfCancellationRequested();
        
        var tasks = formedMatch.UserIds
            .Select(u => _db.StringSetAsync($"{USER_MATCHES_KEY}:{u}", formedMatch.MatchId, _ttl));
        
        await Task.WhenAll(tasks).WaitAsync(cancellationToken);
    }

    public async Task<FormedMatch?> GetUserMatch(string userId)
    {
        var matchId = await _db.StringGetAsync($"{USER_MATCHES_KEY}:{userId}");
        if (matchId.IsNullOrEmpty)
            return null;

        var userSetKey = $"{FORMED_MATCHES_KEY}:{matchId}";
        var userIds = await _db.SetMembersAsync(userSetKey);

        return userIds.Length == 0
            ? null
            : new FormedMatch(matchId!, userIds.Select(v => v.ToString()).ToList());
    }
}
