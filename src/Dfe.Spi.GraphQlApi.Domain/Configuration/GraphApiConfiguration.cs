namespace Dfe.Spi.GraphQlApi.Domain.Configuration
{
    public class GraphApiConfiguration
    {
        public RegistryConfiguration Registry { get; set; }
        public EntityRepositoryConfiguration EntityRepository { get; set; }
        public EnumerationRepositoryConfiguration EnumerationRepository { get; set; }
    }
}