using Application.Common.Interfaces;
using Application.Common.Models.Cdc;
using Domain.Elastic.IRepositories;
using Domain.ElasticAggregationModels;
using Domain.Indices;
using Infrastructure.Elastic.Extensions;
using Nest;
using System.Diagnostics.Metrics;

namespace Infrastructure.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly IErrorLogRepository _errorLogRepository;

        public ErrorLogService(IErrorLogRepository errorLogRepository)
        {
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ICollection<IndexErrorLog>> GetAllAsync()
        {
            var result = await _errorLogRepository.GetAllAsync();

            return result.ToList();
        }

        public async Task<ICollection<IndexErrorLog>> GetByDescriptionMatch(string description)
        {
            //case insensitive
            var query = new QueryContainerDescriptor<IndexErrorLog>().Match(p => p.Field(f => f.Description).Query(description));
            var result = await _errorLogRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexErrorLog>> GetByErrorTitleAndDescriptionMultiMatch(string term)
        {
            var query = new QueryContainerDescriptor<IndexErrorLog>()
                .MultiMatch(p => p.Fields(p => p.Field(f => f.ErrorTitle).Field(d => d.Description)).Query(term).Operator(Operator.And));

            var result = await _errorLogRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexErrorLog>> GetByErrorTitleWithFuzzy(string errorTitle)
        {
            var query = new QueryContainerDescriptor<IndexErrorLog>().Fuzzy(descriptor => descriptor.Field(p => p.ErrorTitle).Value(errorTitle));
            var result = await _errorLogRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexErrorLog>> GetByErrorTitleWithMatch(string errorTitle)
        {
            var query = new QueryContainerDescriptor<IndexErrorLog>().Match(p => p.Field(f => f.ErrorTitle).Query(errorTitle).Operator(Operator.And));
            var result = await _errorLogRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexErrorLog>> GetByErrorTitleWithMatchPhrase(string errorTitle)
        {
            var query = new QueryContainerDescriptor<IndexErrorLog>().MatchPhrase(p => p.Field(f => f.ErrorTitle).Query(errorTitle));
            var result = await _errorLogRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexErrorLog>> GetByErrorTitleWithMatchPhrasePrefix(string errorTitle)
        {
            var query = new QueryContainerDescriptor<IndexErrorLog>().MatchPhrasePrefix(p => p.Field(f => f.ErrorTitle).Query(errorTitle));
            var result = await _errorLogRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexErrorLog>> GetByErrorTitleWithTerm(string errorTitle)
        {
            var query = new QueryContainerDescriptor<IndexErrorLog>().Term(p => p.Field(p => p.ErrorTitle).Value(errorTitle).CaseInsensitive().Boost(6.0));
            var result = await _errorLogRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexErrorLog>> GetByErrorTitleWithWildcard(string errorTitle)
        {
            var query = new QueryContainerDescriptor<IndexErrorLog>().Wildcard(w => w.Field(f => f.ErrorTitle).Value($"*{errorTitle}*").CaseInsensitive());
            var result = await _errorLogRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ErrorLogAggregationModel> GetErrorLogsAggregation()
        {
            var query = new QueryContainerDescriptor<IndexErrorLog>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));

            var result = await _errorLogRepository.SearchAsync(_ => query, a =>
                        a.Sum("ErrorLevel", sa => sa.Field(o => o.ErrorLevel))
                        .Average("AverageErrorLevel", sa => sa.Field(p => p.ErrorLevel)));

            var errorLevel = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "ErrorLevel");
            //var totalMovies = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "TotalAverageErrorLevelMovies");
            var avgErrorLevel = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "AverageErrorLevel");

            return new ErrorLogAggregationModel { ErrorLevel = errorLevel, AverageErrorLevel = avgErrorLevel };
        }

        public async Task<ICollection<IndexErrorLog>> GetErrorLogsAllCondition(string term)
        {
            var query = new QueryContainerDescriptor<IndexErrorLog>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));
            int.TryParse(term, out var numero);

            query = query && new QueryContainerDescriptor<IndexErrorLog>().Wildcard(w => w.Field(f => f.ErrorTitle).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<IndexErrorLog>().Wildcard(w => w.Field(f => f.Description).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<IndexErrorLog>().Term(w => w.ErrorLevel, numero);

            var result = await _errorLogRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async  Task<ICollection<IndexErrorLog>> GetErrorLogsCondition(string errorTitle, string description, DateTime? errorDate)
        {
            QueryContainer query = new QueryContainerDescriptor<IndexErrorLog>();

            if (!string.IsNullOrEmpty(errorTitle))
            {
                query = query && new QueryContainerDescriptor<IndexErrorLog>().MatchPhrasePrefix(qs => qs.Field(fs => fs.ErrorTitle).Query(errorTitle));
            }
            if (!string.IsNullOrEmpty(description))
            {
                query = query && new QueryContainerDescriptor<IndexErrorLog>().MatchPhrasePrefix(qs => qs.Field(fs => fs.Description).Query(description));
            }
            if (errorDate.HasValue)
            {
                query = query && new QueryContainerDescriptor<IndexErrorLog>()
                .Bool(b => b.Filter(f => f.DateRange(dt => dt
                                           .Field(field => field.ErrorDate)
                                           .GreaterThanOrEquals(errorDate)
                                           .LessThanOrEquals(errorDate)
                                           .TimeZone("+00:00"))));
            }

            var result = await _errorLogRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task InsertAsync(IndexErrorLog errorLog)
        {
            await _errorLogRepository.InsertAsync(errorLog);
        }

        public async Task InsertAsync(CdcModel request)
        {

            IndexErrorLog errorLog = new();

            if (request != null && request.Payload != null)
            {
                errorLog.UpdateTime = DateTime.Now;
                errorLog.Id = Guid.NewGuid().ToString();

                if (request.Payload.After != null)
                {
                    errorLog.ErrorDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(request.Payload.After.ErrorDate / 1000d)).ToLocalTime();
                    errorLog.ErrorCorelationId = request.Payload.After.ErrorCorelationId;
                    errorLog.ErrorLevel = request.Payload.After.ErrorLevel;
                    errorLog.ErrorTitle = request.Payload.After.ErrorTitle;
                    errorLog.Description = request.Payload.After.Description;
                }

            }
            await _errorLogRepository.InsertAsync(errorLog);
        }

        public async  Task InsertManyAsync()
        {
            await _errorLogRepository.InsertManyAsync(NestExtensions.GetSampleData());
        }

        public async Task<ICollection<IndexErrorLog>> SearchInAllFiels(string term)
        {
            var query = NestExtensions.BuildMultiMatchQuery<IndexErrorLog>(term);
            var result = await _errorLogRepository.SearchAsync(_ => query);

            return result.ToList();
        }
    }
}
