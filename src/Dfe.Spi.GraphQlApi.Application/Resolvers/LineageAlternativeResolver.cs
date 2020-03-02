using System.Linq;
using System.Threading.Tasks;
using Dfe.Spi.GraphQlApi.Application.GraphTypes;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface ILineageAlternativeResolver : IResolver<LineageAlternativeModel[]>
    {
    }
    
    public class LineageAlternativeResolver : ILineageAlternativeResolver
    {
        public async Task<LineageAlternativeModel[]> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            var lineageEntry = context.Source as LineageEntryModel;
            if (lineageEntry?.Alternatives == null)
            {
                return null;
            }

            return lineageEntry.Alternatives.Select(entry =>
                new LineageAlternativeModel
                {
                    Value = entry.Value?.ToString(),
                    AdapterName = entry.AdapterName,
                    ReadDate = entry.ReadDate,
                }).ToArray();
        }
    }
}