using System;
using System.Threading.Tasks;
using Dfe.Spi.Models.Entities;
using Dfe.Spi.Models.RatesModels;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface IRatesResolver : IResolver<Models.Entities.Rates>
    {
    }

    public class RatesResolver : IRatesResolver
    {
        public async Task<Rates> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            var r = new Random();
            return new Rates
            {
                BaselineFunding = new BaselineFunding
                {
                    Value = r.NextDouble() * 100,
                    PupilCount = r.NextDouble() * 100,
                    BaselineFundingFullSchool = r.Next(),
                    NewAndGrowingSchoolsPupilCountIfFull = r.Next(),
                    NewAndGrowingSchoolsValueIfFull = r.Next(),
                    ValuePerPupil = r.Next(),
                    NewAndGrowingSchoolsValuePerPupilIfFull = r.Next(),
                },
                IllustrativeFunding = new IllustrativeFunding
                {
                    TotalNffFunding = r.Next(),
                    PercentageChangeComparedToBaseline = r.NextDouble() * 100,
                    NewAndGrowingSchoolsTotalNffFundingIfFullyImplemented = r.Next(),
                    TotalNffFundingIfFullyImplemented = r.Next(),
                    PercentageChangeInPupilLedFunding = r.NextDouble() * 100,
                    PercentageChangeComparedToBaselineIfFullyImplemented = r.NextDouble() * 100,
                    PercentageChangeInPupilLedFundingIfFullyImplemented = r.NextDouble() * 100,
                    TotalNffFundingIfFullyImplementedPerPupil = r.Next(),
                    PercentageChangeInPupilLedFundingPerPupil = r.NextDouble() * 100,
                },
                NotionalFunding = new NotionalFunding
                {
                    TotalNffFunding = r.NextDouble() * 100,
                    PercentageChangeComparedToBaseline = r.NextDouble() * 100,
                    PercentageChangeInPupilLedFunding = r.NextDouble() * 100,
                    PupilCount = r.NextDouble() * 100,
                    TotalNffFundingPerPupil = r.NextDouble() * 100,
                    PercentageChangeInPupilLedFundingPerPupil = r.NextDouble() * 100,
                },
            };
        }
    }
}