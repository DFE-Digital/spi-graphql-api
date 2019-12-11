using Dfe.Spi.Models;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class EntityCollection<T> where T : ModelsBase
    {
        public T[] Entities { get; set; }
    }
}