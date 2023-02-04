using Domain.Elastic;
using Domain.Indices;

namespace Domain.Elastic.IRepositories
{
    public interface IErrorLogRepository : IElasticBaseRepository<IndexErrorLog>
    {
    }
}
