namespace Dfe.Spi.GraphQlApi.Domain.Common
{
    public abstract class EntityReference
    {
        public string SourceSystemName { get; set; }
        public string SourceSystemId { get; set; }
    }
}