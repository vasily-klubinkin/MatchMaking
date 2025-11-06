using MatchMaking.Service.Application.Services.Base;
using MatchMaking.Service.Host.Constants;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaking.Service.Host.Controllers;

[ApiController]
[Route("api/v1/matches")]
public class MatchesController : ControllerBase
{
    private readonly IMatchmakingService _matchmakingService;
    private readonly IFormedMatchesService _formedMatchesService;

    public MatchesController(IMatchmakingService matchmakingService, IFormedMatchesService formedMatchesService)
    {
        _matchmakingService = matchmakingService;
        _formedMatchesService = formedMatchesService;
    }

    [HttpPut("search")]
    public async Task<IActionResult> SearchMatchesForUser(
        [FromHeader(Name = HeaderNames.UserIdHeaderName)] string userId,
        CancellationToken cancellationToken)
    {
        await _matchmakingService.SearchMatchForUser(userId, cancellationToken);
        
        return Ok();
    }

    [HttpGet("completed")]
    public async Task<IActionResult> GetUserCompletedMatch([FromHeader(Name = HeaderNames.UserIdHeaderName)] string userId)
    {
        // think about separate POCO for responses on api level
        var userMatch = await _formedMatchesService.Get(userId);
        
        // devopses generally avoid this approach since it makes it harder to monitor failed status codes used as part of application logic)
        if (userMatch is null) return NotFound();
        
        return Ok(userMatch);
    }
}