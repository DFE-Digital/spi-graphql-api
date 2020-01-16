using System.IO;
using System.Linq;
using System.Reflection;
using Dfe.Spi.Common.Logging;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Application.GraphTypes;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Dfe.Spi.GraphQlApi.Functions;
using Dfe.Spi.GraphQlApi.Infrastructure.RegistryApi;
using Dfe.Spi.GraphQlApi.Infrastructure.SearchApi;
using Dfe.Spi.GraphQlApi.Infrastructure.SquasherApi;
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
                };

            LoadAndAddConfiguration(services);
            AddLogging(services);
            AddHttp(services);
            AddSearch(services);
            AddRegistry(services);
            AddEntityRepository(services);
            AddResolvers(services);
            AddGraphQL(services);
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
            services.AddSingleton(_configuration.Search);
            services.AddSingleton(_configuration.Registry);
            services.AddSingleton(_configuration.EntityRepository);
        }

        private void AddLogging(IServiceCollection services)
        {
            services.AddLogging();
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));
            services.AddScoped<ILogger>(provider =>
                provider.GetService<ILoggerFactory>().CreateLogger(LogCategories.CreateFunctionUserCategory("Common")));
            services.AddScoped<ILoggerWrapper, LoggerWrapper>();
        }

        private void AddHttp(IServiceCollection services)
        {
            services.AddTransient<IRestClient, RestClient>();
        }

        private void AddResolvers(IServiceCollection services)
        {
            var resolverType = typeof(IResolver<>);
            var resolvers = resolverType.Assembly.GetTypes().Where(t => t.GetInterface(resolverType.FullName) != null && t.IsClass);
            foreach (var resolver in resolvers)
            {
                var parentResolverInterfaces = resolver.GetInterfaces().Where(t => t.GetInterface(resolverType.FullName)!=null);
                foreach (var parentResolverInterface in parentResolverInterfaces)
                {
                    services.AddScoped(parentResolverInterface, resolver);
                }
            }
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
            services.AddScoped<LearningProviderType>();
            services.AddScoped<SpiQuery>();
            services.AddScoped<SpiSchema>();
        }

        private void AddSearch(IServiceCollection services)
        {
            services.AddScoped<ISearchProvider, SearchApiSearchProvider>();
        }

        private void AddRegistry(IServiceCollection services)
        {
            services.AddScoped<IRegistryProvider, RegistryApiRegistryProvider>();
        }

        private void AddEntityRepository(IServiceCollection services)
        {
            services.AddScoped<IEntityRepository, SquasherEntityRepository>();
        }
    }
}