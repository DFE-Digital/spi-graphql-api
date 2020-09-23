using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Graph;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class SpiSchema : Schema
    {
        private readonly ILoggerWrapper _logger;
        private DataLoaderDocumentListener _dataLoaderDocumentListener;

        public SpiSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<SpiQuery>();

            _dataLoaderDocumentListener = resolver.Resolve<DataLoaderDocumentListener>();
            _logger = resolver.Resolve<ILoggerWrapper>();
        }

        public async Task<string> ExecuteAsync(GraphRequest request)
        {
            _logger.Info($"Executing query {request.Query}");
            
            var result = await this.ExecuteAsync(_ =>
            {
                _.Query = request.Query;
                _.Inputs = request.Variables.ToInputs();
                _.Listeners.Add(_dataLoaderDocumentListener);
            });
            _logger.Info($"Got query result {result}");

            return result;
        }
    }
}