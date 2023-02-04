using Domain.ElasticAggregationModels;
using Domain.Indices;

namespace Application.Services.Abstractions
{
    public  interface IErrorLogService
    {
        Task InsertManyAsync();
        Task InsertAsync(IndexErrorLog errorLog);
        Task<ICollection<IndexErrorLog>> GetAllAsync();
        Task<ICollection<IndexErrorLog>> GetByErrorTitleWithTerm(string errorTitle);
        Task<ICollection<IndexErrorLog>> GetByErrorTitleWithMatch(string errorTitle);
        Task<ICollection<IndexErrorLog>> GetByErrorTitleAndDescriptionMultiMatch(string term);
        Task<ICollection<IndexErrorLog>> GetByErrorTitleWithMatchPhrase(string errorTitle);
        Task<ICollection<IndexErrorLog>> GetByErrorTitleWithMatchPhrasePrefix(string errorTitle);
        Task<ICollection<IndexErrorLog>> GetByErrorTitleWithWildcard(string errorTitle);
        Task<ICollection<IndexErrorLog>> GetByErrorTitleWithFuzzy(string errorTitle);
        Task<ICollection<IndexErrorLog>> SearchInAllFiels(string term);
        Task<ICollection<IndexErrorLog>> GetByDescriptionMatch(string description);
        Task<ICollection<IndexErrorLog>> GetErrorLogsCondition(string errorTitle, string description, DateTime? errorDate);
        Task<ICollection<IndexErrorLog>> GetErrorLogsAllCondition(string term);
        Task<ErrorLogAggregationModel> GetErrorLogsAggregation();
    }
}
