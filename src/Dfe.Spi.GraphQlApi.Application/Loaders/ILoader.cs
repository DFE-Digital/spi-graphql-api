using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dfe.Spi.GraphQlApi.Application.Loaders
{
    public interface ILoader<TKey, TValue>
    {
        Task<IDictionary<TKey, TValue>> LoadAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken);
    }
}