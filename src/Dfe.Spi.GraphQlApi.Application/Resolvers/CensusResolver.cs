using System.Threading.Tasks;
using Dfe.Spi.Models.Entities;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface ICensusResolver : IResolver<Models.Entities.Census>
    {
    }
    
    public class CensusResolver : ICensusResolver
    {
        public Task<Census> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            throw new System.NotImplementedException();
        }
    }
}