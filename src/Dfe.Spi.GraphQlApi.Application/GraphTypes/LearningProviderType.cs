using Dfe.Spi.Models;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class LearningProviderType : ObjectGraphType<LearningProvider>
    {
        public LearningProviderType()
        {
            Field(x => x.Name)
                .Name("name")
                .Description("Name of the learning provider");
            
            Field(x => x.LegalName)
                .Name("legalName")
                .Description("Legal name of the learning provider");
            
            Field(x => x.Urn)
                .Name("urn")
                .Description("URN of the learning provider");
            
            Field(x => x.Ukprn)
                .Name("ukprn")
                .Description("UKPRN of the learning provider");
            
            Field(x => x.Postcode)
                .Name("postcode")
                .Description("Postcode of the learning provider");
        }
    }
}