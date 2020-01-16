using System.Threading.Tasks;

namespace Dfe.Spi.GraphQlApi.Domain.Registry
{
    public interface IRegistryProvider
    {
        Task<object> GetSynonyms(string entityType, string sourceSystem, string sourceSystemId);
    }
}