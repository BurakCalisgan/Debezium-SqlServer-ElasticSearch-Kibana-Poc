using Application.Common.Interfaces;
using Application.Common.Models.Cdc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace Infrastructure.Services.Kafka
{
    public class KafkaConsumerService : BackgroundService
    {
        private IEventConsumer _consumer;
        private readonly IErrorLogService _errorLogService;

        public KafkaConsumerService(IEventConsumer consumer, IErrorLogService errorLogService)
        {
            _consumer = consumer;
            _errorLogService = errorLogService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(() =>
                    ConsumeTopic(stoppingToken),
                    stoppingToken,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Current);
        }

        private async Task ConsumeTopic(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.ReadMessage(stoppingToken);

                    if (consumeResult?.Message == null) continue;


                    var cdcResult = JsonConvert.DeserializeObject<CdcModel>(consumeResult.Message.Value);
                    _errorLogService.InsertAsync(cdcResult);

                    Log.Information($"[{consumeResult.Message.Key}] {consumeResult.Topic} - {consumeResult.Message.Value}");
                    Log.Information($"[Kafka Info] {consumeResult.Topic} - {consumeResult.TopicPartitionOffset}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
            }

        }


    }
}
