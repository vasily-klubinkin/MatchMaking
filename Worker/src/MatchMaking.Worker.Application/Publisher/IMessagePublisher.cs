namespace MatchMaking.Worker.Application.Publisher;

public interface IMessagePublisher<T>
{
    Task PublishAsync(T message, CancellationToken cancellationToken);
}