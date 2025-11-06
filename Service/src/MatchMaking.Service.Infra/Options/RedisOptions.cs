using System.ComponentModel.DataAnnotations;

namespace MatchMaking.Service.Infra.Options;

public class RedisOptions
{
    public const string SectionName = "Redis"; 
    
    [Required]
    public required string ConnectionString { get; init; }
}