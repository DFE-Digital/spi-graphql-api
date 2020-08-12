using Dfe.Spi.GraphQlApi.Application.GraphTypes.Enums;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class LearningProvider : ObjectGraphType<Models.Entities.LearningProvider>
    {
        public LearningProvider(
            IManagementGroupResolver managementGroupResolver,
            ILineageResolver lineageResolver,
            ICensusResolver censusResolver,
            ILearningProviderRatesResolver learningProviderRatesResolver)
        {
            Field(x => x.Name, nullable: true)
                .Name("name")
                .Description("Name");

            Field(x => x.AdministrativeWardCode, nullable: true)
                .Name("administrativeWardCode")
                .Description("Administrative Ward Code");

            Field(x => x.AdministrativeWardName, nullable: true)
                .Name("administrativeWardName")
                .Description("Administrative Ward Name");

            Field<AdmissionsPolicyEnum>(
                name: "admissionsPolicy",
                description: "Admissions Policy",
                resolve: ctx => ctx.Source.AdmissionsPolicy);

            Field(x => x.BoardersCode, nullable: true)
                .Name("boardersCode")
                .Description("Boarders Code");

            Field(x => x.BoardersName, nullable: true)
                .Name("boardersName")
                .Description("Boarders Name");

            Field(x => x.PruChildcareFacilitiesName, nullable: true)
                .Name("pruChildcareFacilitiesName")
                .Description("Pru Childcare Facilities Name");

            Field(x => x.CloseDate, nullable: true)
                .Name("closeDate")
                .Description("Close Date");

            Field(x => x.OfstedLastInspection, nullable: true)
                .Name("ofstedLastInspection")
                .Description("Ofsted Last Inspection");

            Field(x => x.DioceseCode, nullable: true)
                .Name("dioceseCode")
                .Description("Diocese Code");

            Field(x => x.DioceseName, nullable: true)
                .Name("dioceseName")
                .Description("Diocese Name");

            Field(x => x.DistrictAdministrativeCode, nullable: true)
                .Name("districtAdministrativeCode")
                .Description("District Administrative Code");

            Field(x => x.DistrictAdministrativeName, nullable: true)
                .Name("districtAdministrativeName")
                .Description("District Administrative Name");

            Field(x => x.Easting, nullable: true)
                .Name("easting")
                .Description("Easting");

            Field(x => x.PruEbdProvisionCode, nullable: true)
                .Name("pruEbdProvisionCode")
                .Description("Pru Ebd Provision Code");

            Field(x => x.PruEbdProvisionName, nullable: true)
                .Name("pruEbdProvisionName")
                .Description("Pru Ebd Provision Name");

            Field(x => x.PruEducatedByOtherProvidersCode, nullable: true)
                .Name("pruEducatedByOtherProvidersCode")
                .Description("Pru Educated By Other Providers Code");

            Field(x => x.PruEducatedByOtherProvidersName, nullable: true)
                .Name("pruEducatedByOtherProvidersName")
                .Description("Pru Educated By Other Providers Name");

            Field(x => x.EstablishmentNumber, nullable: true)
                .Name("establishmentNumber")
                .Description("Establishment Number");

            Field<LearningProviderStatusEnum>(
                name: "Status",
                description: "Status",
                resolve: ctx => ctx.Source.Status);

            Field<LearningProviderTypeEnum>(
                name: "Type",
                description: "Type",
                resolve: ctx => ctx.Source.Type);

            Field<LearningProviderSubTypeEnum>(
                name: "SubType",
                description: "SubType",
                resolve: ctx => ctx.Source.SubType);

            Field(x => x.FurtherEducationTypeName, nullable: true)
                .Name("furtherEducationTypeName")
                .Description("Further Education Type Name");

            Field(x => x.GenderOfPupilsCode, nullable: true)
                .Name("genderOfPupilsCode")
                .Description("Gender Of Pupils Code");

            Field(x => x.GenderOfPupilsName, nullable: true)
                .Name("genderOfPupilsName")
                .Description("Gender Of Pupils Name");

            Field(x => x.GovernmentOfficeRegionCode, nullable: true)
                .Name("governmentOfficeRegionCode")
                .Description("Government Office Region Code");

            Field(x => x.GovernmentOfficeRegionName, nullable: true)
                .Name("governmentOfficeRegionName")
                .Description("Government Office Region Name");

            Field(x => x.GovernmentStatisticalServiceLocalAuthorityCodeName, nullable: true)
                .Name("governmentStatisticalServiceLocalAuthorityCodeName")
                .Description("Government Statistical Service Local Authority Code Name");

            Field(x => x.InspectorateCode, nullable: true)
                .Name("inspectorateCode")
                .Description("Inspectorate Code");

            Field(x => x.InspectorateName, nullable: true)
                .Name("inspectorateName")
                .Description("Inspectorate Name");

            Field(x => x.LocalAuthorityCode, nullable: true)
                .Name("localAuthorityCode")
                .Description("Local Authority Code");

            Field(x => x.LocalAuthorityName, nullable: true)
                .Name("localAuthorityName")
                .Description("Local Authority Name");

            Field(x => x.LastChangedDate, nullable: true)
                .Name("lastChangedDate")
                .Description("Last Changed Date");

            Field(x => x.MiddleLayerSuperOutputAreaCode, nullable: true)
                .Name("middleLayerSuperOutputAreaCode")
                .Description("Middle Layer Super Output Area Code");

            Field(x => x.MiddleLayerSuperOutputAreaName, nullable: true)
                .Name("middleLayerSuperOutputAreaName")
                .Description("Middle Layer Super Output Area Name");

            Field(x => x.Northing, nullable: true)
                .Name("northing")
                .Description("Northing");

            Field(x => x.NumberOfPupils, nullable: true)
                .Name("numberOfPupils")
                .Description("Number Of Pupils");

            Field(x => x.SixthFormStatusCode, nullable: true)
                .Name("sixthFormStatusCode")
                .Description("Sixth Form Status Code");

            Field(x => x.SixthFormStatusName, nullable: true)
                .Name("sixthFormStatusName")
                .Description("Sixth Form Status Name");

            Field(x => x.OfstedRatingName, nullable: true)
                .Name("ofstedRatingName")
                .Description("Ofsted Rating Name");

            Field(x => x.OpenDate, nullable: true)
                .Name("openDate")
                .Description("Open Date");

            Field(x => x.ParliamentaryConstituencyCode, nullable: true)
                .Name("parliamentaryConstituencyCode")
                .Description("Parliamentary Constituency Code");

            Field(x => x.ParliamentaryConstituencyName, nullable: true)
                .Name("parliamentaryConstituencyName")
                .Description("Parliamentary Constituency Name");

            Field(x => x.PercentageOfPupilsReceivingFreeSchoolMeals, nullable: true)
                .Name("percentageOfPupilsReceivingFreeSchoolMeals")
                .Description("Percentage Of Pupils Receiving Free School Meals");

            Field(x => x.PhaseOfEducationCode, nullable: true)
                .Name("phaseOfEducationCode")
                .Description("Phase Of Education Code");

            Field(x => x.PhaseOfEducationName, nullable: true)
                .Name("phaseOfEducationName")
                .Description("Phase Of Education Name");

            Field(x => x.PruNumberOfPlaces, nullable: true)
                .Name("pruNumberOfPlaces")
                .Description("Pru Number Of Places");

            Field(x => x.Postcode, nullable: true)
                .Name("postcode")
                .Description("Postcode");

            Field(x => x.PreviousEstablishmentNumber, nullable: true)
                .Name("previousEstablishmentNumber")
                .Description("Previous Establishment Number");

            Field(x => x.ClosingReasonCode, nullable: true)
                .Name("closingReasonCode")
                .Description("Closing Reason Code");

            Field(x => x.ClosingReasonName, nullable: true)
                .Name("closingReasonName")
                .Description("Closing Reason Name");

            Field(x => x.OpeningReasonCode, nullable: true)
                .Name("openingReasonCode")
                .Description("Opening Reason Code");

            Field(x => x.OpeningReasonName, nullable: true)
                .Name("openingReasonName")
                .Description("Opening Reason Name");

            Field(x => x.ReligiousEthosCode, nullable: true)
                .Name("religiousEthosCode")
                .Description("Religious Ethos Code");

            Field(x => x.ReligiousEthosName, nullable: true)
                .Name("religiousEthosName")
                .Description("Religious Ethos Name");

            Field(x => x.ResourcedProvisionCapacity, nullable: true)
                .Name("resourcedProvisionCapacity")
                .Description("Resourced Provision Capacity");

            Field(x => x.ResourcedProvisionNumberOnRoll, nullable: true)
                .Name("resourcedProvisionNumberOnRoll")
                .Description("Resourced Provision Number On Roll");

            Field(x => x.RegionalSchoolsCommissionerRegionCode, nullable: true)
                .Name("regionalSchoolsCommissionerRegionCode")
                .Description("Regional Schools Commissioner Region Code");

            Field(x => x.RegionalSchoolsCommissionerRegionName, nullable: true)
                .Name("regionalSchoolsCommissionerRegionName")
                .Description("Regional Schools Commissioner Region Name");

            Field(x => x.SchoolCapacity, nullable: true)
                .Name("schoolCapacity")
                .Description("School Capacity");

            Field(x => x.Website, nullable: true)
                .Name("website")
                .Description("Website");

            Field(x => x.Section41ApprovedCode, nullable: true)
                .Name("section41ApprovedCode")
                .Description("Section41 Approved Code");

            Field(x => x.Section41ApprovedName, nullable: true)
                .Name("section41ApprovedName")
                .Description("Section41 Approved Name");

            Field(x => x.SpecialClassesCode, nullable: true)
                .Name("specialClassesCode")
                .Description("Special Classes Code");

            Field(x => x.SpecialClassesName, nullable: true)
                .Name("specialClassesName")
                .Description("Special Classes Name");

            Field(x => x.HighestAge, nullable: true)
                .Name("highestAge")
                .Description("Highest Age");

            Field(x => x.LowestAge, nullable: true)
                .Name("lowestAge")
                .Description("Lowest Age");

            Field(x => x.TeenageMotherProvisionCode, nullable: true)
                .Name("teenageMotherProvisionCode")
                .Description("Teenage Mother Provision Code");

            Field(x => x.TeenageMotherProvisionName, nullable: true)
                .Name("teenageMotherProvisionName")
                .Description("Teenage Mother Provision Name");

            Field(x => x.TeenageMotherPlaces, nullable: true)
                .Name("teenageMotherPlaces")
                .Description("Teenage Mother Places");

            Field(x => x.TelephoneNumber, nullable: true)
                .Name("telephoneNumber")
                .Description("Telephone Number");

            Field(x => x.AcademyTrustCode, nullable: true)
                .Name("academyTrustCode")
                .Description("Academy Trust Code");

            Field(x => x.AcademyTrustName, nullable: true)
                .Name("academyTrustName")
                .Description("Academy Trust Name");

            Field(x => x.Ukprn, nullable: true)
                .Name("ukprn")
                .Description("Ukprn");

            Field(x => x.Uprn, nullable: true)
                .Name("uprn")
                .Description("Uprn");

            Field(x => x.UrbanRuralCode, nullable: true)
                .Name("urbanRuralCode")
                .Description("Urban Rural Code");

            Field(x => x.UrbanRuralName, nullable: true)
                .Name("urbanRuralName")
                .Description("Urban Rural Name");

            Field(x => x.Urn, nullable: true)
                .Name("urn")
                .Description("Urn");
            
            Field(x => x.CompaniesHouseNumber, nullable: true)
                .Name("companiesHouseNumber")
                .Description("Companies House Number");

            Field(x => x.CharitiesCommissionNumber, nullable: true)
                .Name("charitiesCommissionNumber")
                .Description("Charities Commission Number");

            Field(x => x.DfeNumber, nullable: true)
                .Name("dfeNumber")
                .Description("Dfe Number");

            Field(x => x.LowerLayerSuperOutputAreaCode, nullable: true)
                .Name("lowerLayerSuperOutputAreaCode")
                .Description("Lower Layer Super Output Area Code");

            Field(x => x.LowerLayerSuperOutputAreaName, nullable: true)
                .Name("lowerLayerSuperOutputAreaName")
                .Description("Lower Layer Super Output Area Name");

            Field(x => x.InspectionDate, nullable: true)
                .Name("inspectionDate")
                .Description("Inspection Date");

            Field(x => x.InspectorateReport, nullable: true)
                .Name("inspectorateReport")
                .Description("Inspectorate Report");

            Field(x => x.LegalName, nullable: true)
                .Name("legalName")
                .Description("Legal Name");

            Field(x => x.ContactEmail, nullable: true)
                .Name("contactEmail")
                .Description("Contact Email");

            Field(x => x.AddressLine1, nullable: true)
                .Name("addressLine1")
                .Description("Address Line1");

            Field(x => x.AddressLine2, nullable: true)
                .Name("addressLine2")
                .Description("Address Line2");

            Field(x => x.AddressLine3, nullable: true)
                .Name("addressLine3")
                .Description("Address Line3");

            Field(x => x.Town, nullable: true)
                .Name("town")
                .Description("Town");

            Field(x => x.County, nullable: true)
                .Name("county")
                .Description("County");


            //////////////////////////////////////////////////////////////////////////////////////
            /// Sub-objects
            //////////////////////////////////////////////////////////////////////////////////////

            Field<ListGraphType<LineageEntry>>("_lineage",
                resolve: lineageResolver.ResolveAsync);

            Field<ManagementGroup>("managementGroup",
                resolve: managementGroupResolver.ResolveAsync,
                arguments: new QueryArguments(new QueryArgument[]
                {
                    new QueryArgument<DateGraphType> {Name = "pointInTime"},
                }));

            Field<Census>("census",
                resolve: censusResolver.ResolveAsync,
                arguments: new QueryArguments(new QueryArgument[]
                {
                    new QueryArgument<IntGraphType> {Name = "year"},
                    new QueryArgument<StringGraphType> {Name = "type"},
                }));

            Field<LearningProviderRates>("rates",
                resolve: learningProviderRatesResolver.ResolveAsync,
                arguments: new QueryArguments(new QueryArgument[]
                {
                    new QueryArgument<IntGraphType> {Name = "year"},
                }));
        }
    }
}