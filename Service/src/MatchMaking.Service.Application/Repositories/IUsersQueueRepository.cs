namespace MatchMaking.Service.Application.Repositories;

public interface IUsersQueueRepository
{
    Task<bool> IsUserInQueue(string userId);

    Task AddUserToQueue(string userId);
    
    Task ClearUsersInQueue(ICollection<string> userIds);
}