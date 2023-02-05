using Application.Common.Interfaces;
using Application.Common.Models.Kafka;
using Confluent.Kafka;
using Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Application.Kafka
{
    public class KafkaConsumer : IEventConsumer
    {
        private readonly KafkaConfiguration _kafkaConfiguration;
        private IConsumer<string, string> _consumer;

        public KafkaConsumer(IOptions<KafkaConfiguration> kafkaConfigurationOptions)
        {
            _kafkaConfiguration = kafkaConfigurationOptions?.Value ?? throw new ArgumentException(nameof(kafkaConfigurationOptions));
            _consumer = CreateConsumer();
        }

        public ConsumeResult<string, string> ReadMessage(CancellationToken cancellationToken)
        {
            return _consumer.Consume(cancellationToken);
        }

        private IConsumer<string, string> CreateConsumer()
        {
            var config = new ConsumerConfig()
            {
                BootstrapServers = _kafkaConfiguration.Brokers,
                GroupId = _kafkaConfiguration.ConsumerGroup,
                SecurityProtocol = SecurityProtocol.Plaintext,
                EnableAutoCommit = _kafkaConfiguration.AutoCommit,
                StatisticsIntervalMs = _kafkaConfiguration.StatisticsIntervalMs,
                SessionTimeoutMs = _kafkaConfiguration.SessionTimeoutMs,
                MaxPollIntervalMs = _kafkaConfiguration.MaxPollIntervalMs,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true,
                AutoCommitIntervalMs = 5000,
                EnableAutoOffsetStore = true,
            };

            _consumer = new ConsumerBuilder<string, string>(config).SetStatisticsHandler((_, kafkaStatistics) => LogKafkaStats(kafkaStatistics)).
                SetErrorHandler((_, e) => LogKafkaError(e)).Build();
            _consumer.Subscribe(_kafkaConfiguration.Topic);

            return _consumer;
        }

        private void LogKafkaStats(string kafkaStatistics)
        {
            var stats = JsonConvert.DeserializeObject<KafkaStatistics>(kafkaStatistics);

            if (stats?.topics != null && stats.topics.Count > 0)
            {
                foreach (var topic in stats.topics)
                {
                    foreach (var partition in topic.Value.Partitions)
                    {
                        Task.Run(() =>
                        {
                            var logMessage = $"FxRates:KafkaStats Topic: {topic.Key} Partition: {partition.Key} PartitionConsumerLag: {partition.Value.ConsumerLag}";
                            Log.Information(logMessage);
                        });
                    }
                }
            }
        }

        private void LogKafkaError(Error ex)
        {
            Task.Run(() =>
            {
                var error = $"Kafka Exception: ErrorCode:[{ex.Code}] Reason:[{ex.Reason}] Message:[{ex.ToString()}]";
                Log.Error(error);
            });
        }

    }
}
