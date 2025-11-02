using System.ComponentModel.DataAnnotations;

namespace MatchMaking.Worker.Application.Options;

public class MatchOptions
{
    public const string SectionName = "MatchOptions";
    
    [Required]
    public int UsersRequired { get; init; }
    
    // todo: add a timestamp field to define the maximum waiting time for a matchmaking request
    // this will allow skipping messages that have been sitting in the queue for too long —
    // useful for recovering faster in case of system lag, backlog buildup or technical problems
}