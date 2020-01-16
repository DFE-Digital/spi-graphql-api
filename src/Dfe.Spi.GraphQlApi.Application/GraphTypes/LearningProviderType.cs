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

            Field(x => x.LegalName, nullable:true)
                .Name("legalName")
                .Description("Legal name of the learning provider");
            
            Field(x => x.Urn, nullable:true)
                .Name("urn")
                .Description("URN of the learning provider");
            
            Field(x => x.Ukprn, nullable:true)
                .Name("ukprn")
                .Description("UKPRN of the learning provider");
            
            Field(x => x.Postcode, nullable:true)
                .Name("postcode")
                .Description("Postcode of the learning provider");
        }
    }
}