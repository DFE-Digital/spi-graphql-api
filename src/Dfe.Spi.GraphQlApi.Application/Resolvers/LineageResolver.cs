using System.Linq;
using System.Threading.Tasks;
using Dfe.Spi.GraphQlApi.Application.GraphTypes;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface ILineageResolver : IResolver<LineageEntryModel[]>
    {
    }
    
    public class LineageResolver : ILineageResolver
    {
        public async Task<LineageEntryModel[]> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            var sourceEntity = context.Source as Models.Entities.EntityBase;
            if (sourceEntity?._Lineage == null)
            {
                return null;
            }

            return sourceEntity._Lineage.Select(kvp =>
                new LineageEntryModel
                {
                    Name = kvp.Key,
                    Value = kvp.Value.Value?.ToString(),
                    AdapterName = kvp.Value.AdapterName,
                    ReadDate = kvp.Value.ReadDate,
                    Alternatives = kvp.Value.Alternatives.Select(a=>
                        new LineageAlternativeModel
                        {
                            Value = a.Value?.ToString(),
                            AdapterName = a.AdapterName,
                            ReadDate = a.ReadDate,
                        }).ToArray()
                }).ToArray();
        }
    }
}