namespace MatchMaking.Service.Application.POCO;

// Represents a set of user attributes used to group players into matchmaking queues.
// Users within the same group CAN be matched with each other,
// while users from different groups CANNOT be matched together.
public record UserProps(string Region);