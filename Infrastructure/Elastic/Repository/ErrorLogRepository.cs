using Domain.Elastic.IRepositories;
using Domain.Indices;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Elastic.Repository
{
    public class ErrorLogRepository : ElasticBaseRepository<IndexErrorLog>, IErrorLogRepository
    {
        public ErrorLogRepository(IElasticClient elasticClient) : base(elasticClient)
        {
        }

        public override string IndexName { get; } = "indexerrorlogs";
    }
}
