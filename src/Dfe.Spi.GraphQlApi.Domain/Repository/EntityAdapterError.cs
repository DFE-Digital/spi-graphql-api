using Dfe.Spi.Common.Models;

namespace Dfe.Spi.GraphQlApi.Domain.Repository
{
    public class EntityAdapterError
    {
        public string AdapterName { get; set; }
        public string RequestedEntityName { get; set; }
        public string RequestedId { get; set; }
        public string[] RequestedFields { get; set; }
        public int HttpStatusCode { get; set; }
        public HttpErrorBody HttpErrorBody { get; set; }
    }
}