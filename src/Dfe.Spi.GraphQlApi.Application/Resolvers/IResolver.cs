using System.Threading.Tasks;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface IResolver<T>
    {
        Task<T[]> ResolveAsync<TContext>(ResolveFieldContext<TContext> context);
    }
}