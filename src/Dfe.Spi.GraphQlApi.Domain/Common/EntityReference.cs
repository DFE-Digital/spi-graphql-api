namespace Dfe.Spi.GraphQlApi.Domain.Common
{
    public class EntityReference
    {
        public string SourceSystemName { get; set; }
        public string SourceSystemId { get; set; }
    }
    public class TypedEntityReference : EntityReference
    {
        public string EntityType { get; set; }
    }
}