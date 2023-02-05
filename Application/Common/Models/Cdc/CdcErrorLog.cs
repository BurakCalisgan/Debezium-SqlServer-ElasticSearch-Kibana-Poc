using Confluent.Kafka;

namespace Application.Common.Models.Cdc
{
    public class CdcErrorLog
    {
        public int Id { get; set; }
        public string ErrorTitle { get; set; }
        public string Description { get; set; }
        public Int64 ErrorDate { get; set; }
        public string ErrorCorelationId { get; set; }
        public int ErrorLevel { get; set; }
    }
}
