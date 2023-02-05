using Confluent.Kafka;
namespace Application.Common.Interfaces
{
    public interface IEventConsumer
    {
        ConsumeResult<string, string> ReadMessage(CancellationToken token);
    }
}
