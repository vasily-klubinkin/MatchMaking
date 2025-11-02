namespace MatchMaking.Worker.Application.POCO;


// QueueId is used to isolate users and enable parallel processing.
// In practice, it corresponds to a Kafka topic partition, and user properties
// (currently just region, e.g., "us-west") are used to distribute users between partitions.
// The ideal degree of parallelism is reached when the number of partitions matches
// all possible combinations of UserProps.
public record MatchmakingRequest(string UserId, int QueueId);