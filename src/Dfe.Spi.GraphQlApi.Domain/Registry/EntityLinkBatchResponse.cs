using Dfe.Spi.GraphQlApi.Domain.Common;

namespace Dfe.Spi.GraphQlApi.Domain.Registry
{
    public class EntityLinkBatchResult
    {
        public TypedEntityReference Entity { get; set; }
        public EntityLinkReference[] Links { get; set; }
    }
}