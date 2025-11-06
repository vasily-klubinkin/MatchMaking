using System.ComponentModel.DataAnnotations;

namespace MatchMaking.Worker.Infra.Options;

public class KafkaOptions
{
    public const string SectionName = "Kafka";

    [Required]
    public required string BootstrapServers { get; init; } = null!;

    [Required]
    public required string ConsumerGroup { get; init; } = null!;

    [Required]
    public required string MatchmakingRequestTopic { get; init; } = null!;

    [Required]
    public required string MatchmakingCompleteTopic { get; init; } = null!;

    public Dictionary<string, string>? ConsumerConfig { get; init; }

    public Dictionary<string, string>? ProducerConfig { get; init; }
}