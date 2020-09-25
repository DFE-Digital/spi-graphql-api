using System;
using Dfe.Spi.GraphQlApi.Domain.Common;

namespace Dfe.Spi.GraphQlApi.Infrastructure.RegistryApi
{
    public class GetLinksBatchRequest
    {
        public TypedEntityReference[] Entities { get; set; }
        public DateTime? PointInTime { get; set; }
    }
}
