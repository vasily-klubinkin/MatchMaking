using System.ComponentModel.DataAnnotations;

namespace MatchMaking.Worker.Infra.Options;

public class KafkaOptions
{
    public const string SectionName = "Kafka";

    [Required]
    public string BootstrapServers { get; init; } = null!;

    [Required]
    public string ConsumerGroup { get; init; } = null!;

    [Required]
    public string MatchmakingRequestTopic { get; init; } = null!;

    [Required]
    public string MatchmakingCompleteTopic { get; init; } = null!;

    public Dictionary<string, string>? ConsumerConfig { get; init; }

    public Dictionary<string, string>? ProducerConfig { get; init; }
}