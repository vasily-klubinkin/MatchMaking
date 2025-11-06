namespace MatchMaking.Service.Application.Publisher;

public interface IMessagePublisher<T>
{
    Task PublishAsync(T message, CancellationToken cancellationToken);
}