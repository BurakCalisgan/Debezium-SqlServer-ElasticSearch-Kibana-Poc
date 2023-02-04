using Domain.Indices;
using Nest;

namespace Infrastructure.Elastic.Extensions
{
    public static class NestExtensions
    {
        public static QueryContainer BuildMultiMatchQuery<T>(string queryValue) where T : class
        {
            var fields = typeof(T).GetProperties().Select(p => p.Name.ToLower()).ToArray();

            return new QueryContainerDescriptor<T>()
                .MultiMatch(c => c
                    .Type(TextQueryType.Phrase)
                    .Fields(f => f.Fields(fields)).Lenient().Query(queryValue));
        }

        public static List<IndexErrorLog> GetSampleData()
        {
            var list = new List<IndexErrorLog>
            {
                new() {Id = "38d61273-8f4e-431c-a0bb-3de2fbbf8755", ErrorTitle = "Not Found Exception", Description= "Lorem Ipsum", ErrorCorelationId = Guid.NewGuid().ToString(), ErrorDate = new DateTime(2023, 1, 03), ErrorLevel= 5, UpdateTime = DateTime.Now},
                new() {Id = "38d61273-8f4e-431c-a0bb-3de2fbbf8756", ErrorTitle = "Not Found Exception", Description= "Lorem Ipsum2", ErrorCorelationId = Guid.NewGuid().ToString(), ErrorDate = new DateTime(2023, 1, 03), ErrorLevel= 5, UpdateTime = DateTime.Now},
                new() {Id = "38d61273-8f4e-431c-a0bb-3de2fbbf8757", ErrorTitle = "Not Found Exception", Description= "Lorem Ipsum3", ErrorCorelationId = Guid.NewGuid().ToString(), ErrorDate = new DateTime(2023, 1, 03), ErrorLevel= 5, UpdateTime = DateTime.Now},
                new() {Id = "38d61273-8f4e-431c-a0bb-3de2fbbf8758", ErrorTitle = "Not Found Exception", Description= "Lorem Ipsum4", ErrorCorelationId = Guid.NewGuid().ToString(), ErrorDate = new DateTime(2023, 1, 03), ErrorLevel= 5, UpdateTime = DateTime.Now}
            };
            return list;
        }

        public static double ObterBucketAggregationDouble(AggregateDictionary agg, string bucket)
        {
            return agg.BucketScript(bucket).Value.HasValue ? agg.BucketScript(bucket).Value.Value : 0;
        }
    }
}
