using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Core.Extensions
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultIndex = configuration["ElasticsearchSettings:DefaultIndex"];
            var basicAuthUser = configuration["ElasticsearchSettings:Username"];
            var basicAuthPassword = configuration["ElasticsearchSettings:Password"];

            var settings = new ConnectionSettings(new Uri(configuration["ElasticsearchSettings:Uri"]));

            if (!string.IsNullOrEmpty(defaultIndex))
                settings = settings.DefaultIndex(defaultIndex);

            if (!string.IsNullOrEmpty(basicAuthUser) && !string.IsNullOrEmpty(basicAuthPassword))
                settings = settings.BasicAuthentication(basicAuthUser, basicAuthPassword);

            settings.EnableApiVersioningHeader();

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }
    }
   
}
