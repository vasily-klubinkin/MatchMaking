namespace MatchMaking.Worker.Application.Repositories;

public interface IUserQueuesRepository
{
    Task<int> GetUsersCountAsync(int queueId);

    Task<ICollection<string>> DequeueUsersAsync(int queueId, int count);
    
    Task EnqueueUserAsync(int queueId, string userId);
}