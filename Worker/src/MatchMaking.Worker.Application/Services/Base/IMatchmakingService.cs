using MatchMaking.Worker.Application.POCO;

namespace MatchMaking.Worker.Application.Services.Base;

public interface IMatchmakingService
{
    Task<FormedMatch?> HandleMatchmakingRequestAsync(MatchmakingRequest request);
}