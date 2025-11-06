namespace MatchMaking.Service.Application.POCO;

// could be moved to nuget or we can merge solutions into single solution are move this to "contracts"
public record FormedMatch(string MatchId, ICollection<string> UserIds);