using System.Threading;
using System.Threading.Tasks;

namespace Dfe.Spi.GraphQlApi.Domain.Enumerations
{
    public interface IEnumerationRepository
    {
        Task<string[]> GetEnumerationValuesAsync(string enumName, CancellationToken cancellationToken);
    }
}