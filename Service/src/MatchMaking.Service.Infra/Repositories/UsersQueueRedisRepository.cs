using MatchMaking.Service.Application.Repositories;
using StackExchange.Redis;

// todo: add some time limits to redis(cause matchmaking is realtime and we need to find match
// no longer than specific timeframe (like no longer than 5 min) we can set ttl to 5 min
// to avoid of stucking users in the queue 
namespace MatchMaking.Service.Infra.Repositories;

internal class UsersQueueRedisRepository : IUsersQueueRepository
{
    private readonly IDatabase _db;

    public UsersQueueRedisRepository(IConnectionMultiplexer multiplexer)
    {
        _db = multiplexer.GetDatabase();
    }

    public Task<bool> IsUserInQueue(string userId) =>
        _db.SetContainsAsync("users_queue", userId);

    public Task AddUserToQueue(string userId) =>
        _db.SetAddAsync("users_queue", userId);

    public Task ClearUsersInQueue(ICollection<string> userIds)
    {
        var tasks = userIds.Select(u => _db.SetRemoveAsync("users_queue", u));
        return Task.WhenAll(tasks);
    }
}