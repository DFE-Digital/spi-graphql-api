using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class BaselineFunding : ObjectGraphType<Models.RatesModels.BaselineFunding>
    {
        public BaselineFunding()
        {
            Field(x => x.Value, nullable: true)
                .Name("value")
                .Description("Value");
            
            Field(x => x.PupilCount, nullable: true)
                .Name("pupilCount")
                .Description("Pupil count");
            
            Field(x => x.BaselineFundingFullSchool, nullable: true)
                .Name("baselineFundingFullSchool")
                .Description("Baseline funding full school");
            
            Field(x => x.NewAndGrowingSchoolsPupilCountIfFull, nullable: true)
                .Name("newAndGrowingSchoolsPupilCountIfFull")
                .Description("New and growing schools pupil count if full");
            
            Field(x => x.NewAndGrowingSchoolsValueIfFull, nullable: true)
                .Name("newAndGrowingSchoolsValueIfFull")
                .Description("New and growing schools value if full");
            
            Field(x => x.ValuePerPupil, nullable: true)
                .Name("valuePerPupil")
                .Description("Value per pupil");
            
            Field(x => x.NewAndGrowingSchoolsValuePerPupilIfFull, nullable: true)
                .Name("newAndGrowingSchoolsValuePerPupilIfFull")
                .Description("New and growing schools value per pupil if full");
        }
    }
}