using System.Collections.Concurrent;
using MatchMaking.Worker.Application.Repositories;

namespace MatchMaking.Worker.Infra.Repositories;

internal class UserQueuesInMemoryRepository : IUserQueuesRepository
{
    private readonly ConcurrentDictionary<int, ConcurrentQueue<string>> _queues = new();
    private readonly ConcurrentDictionary<string, byte> _allUsers = new(); // acts like a concurrent bitmap

    public Task<int> GetUsersCountAsync(int queueId)
    {
        return Task.FromResult(_queues.TryGetValue(queueId, out var queue) ? queue.Count : 0);
    }

    public Task<ICollection<string>> DequeueUsersAsync(int queueId, int count)
    {
        if (!_queues.TryGetValue(queueId, out var queue)) return Task.FromResult<ICollection<string>>(Array.Empty<string>());

        var users = new List<string>(count);
        
        while (users.Count < count && queue.TryDequeue(out var userId))
        {
            _allUsers.TryRemove(userId, out _);
            users.Add(userId);
        }

        return Task.FromResult<ICollection<string>>(users);
    }

    public Task EnqueueUserAsync(int queueId, string userId)
    {
        // Avoid duplicates globally
        if (!_allUsers.TryAdd(userId, 0))
            return Task.CompletedTask;

        var queue = _queues.GetOrAdd(queueId, _ => new ConcurrentQueue<string>());
        queue.Enqueue(userId);

        return Task.CompletedTask;
    }
}