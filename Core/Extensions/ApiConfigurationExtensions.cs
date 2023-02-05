using Application.Common.Interfaces;
using Application.Kafka;
using Core.Middleware;
using Domain.Elastic.IRepositories;
using Infrastructure.Configurations;
using Infrastructure.Elastic.Repository;
using Infrastructure.Services;
using Infrastructure.Services.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Core.Extensions
{
    public static class ApiConfigurationExtensions
    {
        public static void AddApiConfiguration(this IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddTransient<IErrorLogService, ErrorLogService>();
            services.AddTransient<IErrorLogRepository, ErrorLogRepository>();
            services.AddSingleton<IEventConsumer, KafkaConsumer>();

            services.AddHostedService<KafkaConsumerService>();

            services.AddControllers();
        }

        public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = SerilogExtensions.EnrichFromRequest);

            app.UseMiddleware<RequestSerilLogMiddleware>();
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
        }
    }
}
