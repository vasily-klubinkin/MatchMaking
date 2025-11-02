using System.ComponentModel.DataAnnotations;

namespace MatchMaking.Service.Infra.Options;

public class KafkaOptions
{
    public const string SectionName = "Kafka";
    
    [Required]
    public string BootstrapServers { get; init; }
    
    [Required]
    public string MatchmakingRequestsTopic { get; init; }

    [Required]
    public string CompletedMatchesTopic { get; init; }
    
    [Required]
    public string ConsumerGroup { get; set; }
    
    public Dictionary<string, string>? ConsumerConfig { get; init; }

    public Dictionary<string, string>? ProducerConfig { get; init; }
}