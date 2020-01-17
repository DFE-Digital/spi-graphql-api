namespace Dfe.Spi.GraphQlApi.Domain.Configuration
{
    public class GraphApiConfiguration
    {
        public SearchConfiguration Search { get; set; }
        public RegistryConfiguration Registry { get; set; }
        public EntityRepositoryConfiguration EntityRepository { get; set; }
    }
}