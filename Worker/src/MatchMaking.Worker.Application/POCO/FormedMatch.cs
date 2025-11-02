namespace MatchMaking.Worker.Application.POCO;

public record FormedMatch(string MatchId, ICollection<string> UserIds);