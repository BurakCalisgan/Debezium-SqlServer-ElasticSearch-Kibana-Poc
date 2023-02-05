using Newtonsoft.Json;

namespace Application.Common.Models.Kafka
{
    public class TopicData
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("partitions")]
        public IReadOnlyDictionary<string, PartitionData> Partitions { get; set; }
    }
}
