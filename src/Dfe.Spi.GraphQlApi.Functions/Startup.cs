using System.IO;
using System.Linq;
using Dfe.Spi.Common.Caching;
using Dfe.Spi.Common.Caching.Definitions;
using Dfe.Spi.Common.Context.Definitions;
using Dfe.Spi.Common.Http.Server;
using Dfe.Spi.Common.Http.Server.Definitions;
using Dfe.Spi.Common.Logging;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Application.GraphTypes;
using Dfe.Spi.GraphQlApi.Application.GraphTypes.Enums;
using Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Dfe.Spi.GraphQlApi.Domain.Enumerations;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.GraphQlApi.Functions;
using Dfe.Spi.GraphQlApi.Infrastructure.RegistryApi;
using Dfe.Spi.GraphQlApi.Infrastructure.SquasherApi;
using Dfe.Spi.GraphQlApi.Infrastructure.TranslatorApi;
using GraphQL;
using GraphQL.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Dfe.Spi.GraphQlApi.Functions
{
    public class Startup : FunctionsStartup
    {
        private IConfigurationRoot _rawConfiguration;
        private GraphApiConfiguration _configuration;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            // Setup JSON serialization
            JsonConvert.DefaultSettings =
                () => new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                };

            LoadAndAddConfiguration(services);
            AddLogging(services);
            AddHttp(services);
            AddRegistry(services);
            AddEntityRepository(services);
            AddEnumerationRepository(services);
            AddResolvers(services);
            AddGraphQL(services);

            services
                .AddSingleton<ICacheProvider, CacheProvider>();
        }

        private void LoadAndAddConfiguration(IServiceCollection services)
        {
            _rawConfiguration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables(prefix: "SPI_")
                .Build();
            services.AddSingleton(_rawConfiguration);

            _configuration = new GraphApiConfiguration();
            _rawConfiguration.Bind(_configuration);
            services.AddSingleton(_configuration);
            services.AddSingleton(_configuration.Registry);
            services.AddSingleton(_configuration.EntityRepository);
            services.AddSingleton(_configuration.EnumerationRepository);
        }

        private void AddLogging(IServiceCollection services)
        {
            services.AddLogging();
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));
            services.AddScoped<ILogger>(provider =>
                provider.GetService<ILoggerFactory>().CreateLogger(LogCategories.CreateFunctionUserCategory("GraphQl")));

            services.AddScoped<IHttpSpiExecutionContextManager, HttpSpiExecutionContextManager>();
            services.AddScoped<ISpiExecutionContextManager>((provider) =>
                (ISpiExecutionContextManager) provider.GetService(typeof(IHttpSpiExecutionContextManager)));
            services.AddScoped<ILoggerWrapper, LoggerWrapper>();
        }

        private void AddHttp(IServiceCollection services)
        {
            services.AddTransient<IRestClient, RestClient>();
        }

        private void AddResolvers(IServiceCollection services)
        {
            var resolverType = typeof(IResolver<>);
            var resolvers = resolverType.Assembly.GetTypes()
                .Where(t => t.GetInterface(resolverType.FullName) != null && t.IsClass);
            foreach (var resolver in resolvers)
            {
                var parentResolverInterfaces =
                    resolver.GetInterfaces().Where(t => t.GetInterface(resolverType.FullName) != null);
                foreach (var parentResolverInterface in parentResolverInterfaces)
                {
                    services.AddScoped(parentResolverInterface, resolver);
                }
            }

            services.AddScoped<IEnumerationLoader, EnumerationLoader>();
        }

        private void AddGraphQL(IServiceCollection services)
        {
            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver((type) =>
            {
                var instance = s.GetRequiredService(type);
                return instance;
            }));
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            
            // enums
            services.AddScoped<AdmissionsPolicyEnum>();
            services.AddScoped<CensusAggregationFieldsEnum>();
            services.AddScoped<LearningProviderStatusEnum>();
            services.AddScoped<LearningProviderSubTypeEnum>();
            services.AddScoped<LearningProviderTypeEnum>();
            services.AddScoped<OperatorEnum>();
            
            // inputs
            services.AddScoped<ComplexQueryCondition>();
            services.AddScoped<ComplexQueryGroup>();
            services.AddScoped<ComplexQuery>();
            services.AddScoped<Application.GraphTypes.Inputs.AggregationRequestCondition>();
            services.AddScoped<Application.GraphTypes.Inputs.AggregationRequest>();
            
            // entities
            services.AddScoped<Application.GraphTypes.Census>();
            services.AddScoped<Application.GraphTypes.Aggregation>();
            services.AddScoped<Application.GraphTypes.PaginationDetails>();
            services.AddScoped<Application.GraphTypes.ManagementGroupsPaged>();
            services.AddScoped<Application.GraphTypes.ManagementGroup>();
            services.AddScoped<Application.GraphTypes.LearningProvidersPaged>();
            services.AddScoped<Application.GraphTypes.LearningProvider>();
            services.AddScoped<Application.GraphTypes.LineageEntry>();
            services.AddScoped<Application.GraphTypes.LineageAlternativeEntry>();
            services.AddScoped<Application.GraphTypes.LearningProviderRates>();
            services.AddScoped<Application.GraphTypes.BaselineFunding>();
            services.AddScoped<Application.GraphTypes.IllustrativeFunding>();
            services.AddScoped<Application.GraphTypes.NotionalFunding>();
            services.AddScoped<Application.GraphTypes.ManagementGroupProvisionalFunding>();
            services.AddScoped<Application.GraphTypes.ManagementGroupRates>();
            
            // schema
            services.AddScoped<SpiQuery>();
            services.AddScoped<SpiSchema>();
        }

        private void AddRegistry(IServiceCollection services)
        {
            services.AddScoped<IRegistryProvider, RegistryApiRegistryProvider>();
        }

        private void AddEntityRepository(IServiceCollection services)
        {
            services.AddScoped<IEntityRepository, SquasherEntityRepository>();
        }

        private void AddEnumerationRepository(IServiceCollection services)
        {
            services.AddScoped<IEnumerationRepository, TranslatorApiEnumerationRepository>();
        }
    }
}