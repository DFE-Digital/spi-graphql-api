namespace Dfe.Spi.GraphQlApi.Domain.Registry
{
    public class SearchFilter
    {
        public string Field { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
    }
}