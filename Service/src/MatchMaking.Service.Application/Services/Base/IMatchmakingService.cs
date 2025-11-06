namespace MatchMaking.Service.Application.Services.Base;

public interface IMatchmakingService
{
    Task SearchMatchForUser(string userId, CancellationToken cancellationToken);
}