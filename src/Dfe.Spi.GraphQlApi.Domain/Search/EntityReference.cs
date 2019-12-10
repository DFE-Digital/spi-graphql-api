namespace Dfe.Spi.GraphQlApi.Domain.Search
{
    public abstract class EntityReference
    {
        public string SourceSystemName { get; set; }
        public string SourceSystemId { get; set; }
    }
}