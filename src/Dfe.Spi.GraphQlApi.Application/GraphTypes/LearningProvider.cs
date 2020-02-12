using Dfe.Spi.Models;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class LearningProvider : ObjectGraphType<Models.LearningProvider>
    {
        public LearningProvider()
        {
            Field(x => x.Name, nullable: true)
                .Name("name")
                .Description("Name");

            Field(x => x.AcademyTrustCode, nullable: true)
                .Name("academyTrustCode")
                .Description("Academy Trust Code");

            Field(x => x.AdministrativeWardCode, nullable: true)
                .Name("administrativeWardCode")
                .Description("Administrative Ward Code");

            Field(x => x.AdministrativeWardName, nullable: true)
                .Name("administrativeWardName")
                .Description("Administrative Ward Name");

            Field(x => x.AdmissionsPolicy, nullable: true)
                .Name("admissionsPolicy")
                .Description("Admissions Policy");

            Field<BoardersCodeEnum>(
                name: "boardersCode",
                description: "Boarders Code",
                resolve: ctx => ctx.Source.BoardersCode);

            Field(x => x.BoardersName, nullable: true)
                .Name("boardersName")
                .Description("Boarders Name");

            Field(x => x.CharitiesCommissionNumber, nullable: true)
                .Name("charitiesCommissionNumber")
                .Description("Charities Commission Number");

            Field(x => x.CloseDate, nullable: true)
                .Name("closeDate")
                .Description("Close Date");

            Field(x => x.ClosingReason, nullable: true)
                .Name("closingReason")
                .Description("Closing Reason");

            Field(x => x.CompaniesHouseNumber, nullable: true)
                .Name("companiesHouseNumber")
                .Description("Companies House Number");

            Field(x => x.DfeNumber, nullable: true)
                .Name("dfeNumber")
                .Description("Dfe Number");

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

            Field(x => x.EstablishmentNumber, nullable: true)
                .Name("establishmentNumber")
                .Description("Establishment Number");

            Field(x => x.FederationFlag, nullable: true)
                .Name("federationFlag")
                .Description("Federation Flag");

            Field(x => x.FurtherEducationType, nullable: true)
                .Name("furtherEducationType")
                .Description("Further Education Type");

            Field<LearningProviderGenderOfEntryEnum>(
                name: "genderOfEntry",
                description: "Gender Of Entry",
                resolve: ctx => ctx.Source.GenderOfEntry);

            Field(x => x.GorRegion, nullable: true)
                .Name("gorRegion")
                .Description("Gor Region");

            Field(x => x.GsslaCode, nullable: true)
                .Name("gsslaCode")
                .Description("Gssla Code");

            Field(x => x.Id, nullable: true)
                .Name("id")
                .Description("Id");

            Field(x => x.InspectionDate, nullable: true)
                .Name("inspectionDate")
                .Description("Inspection Date");

            Field(x => x.InspectorateName, nullable: true)
                .Name("inspectorateName")
                .Description("Inspectorate Name");

            Field(x => x.InspectorateReport, nullable: true)
                .Name("inspectorateReport")
                .Description("Inspectorate Report");

            Field(x => x.LegalName, nullable: true)
                .Name("legalName")
                .Description("Legal Name");

            Field<LocalAuthorityCodeEnum>(
                name: "LocalAuthorityCode",
                description: "Local Authority Code",
                resolve: ctx => ctx.Source.LocalAuthorityCode);

            Field(x => x.LocalAuthorityName, nullable: true)
                .Name("localAuthorityName")
                .Description("Local Authority Name");

            Field(x => x.ManagementGroupType, nullable: true)
                .Name("managementGroupType")
                .Description("Management Group Type");

            Field(x => x.Northing, nullable: true)
                .Name("northing")
                .Description("Northing");

            Field(x => x.OfstedLastInspection, nullable: true)
                .Name("ofstedLastInspection")
                .Description("Ofsted Last Inspection");

            Field(x => x.OfstedRating, nullable: true)
                .Name("ofstedRating")
                .Description("Ofsted Rating");

            Field(x => x.OpenDate, nullable: true)
                .Name("openDate")
                .Description("Open Date");

            Field(x => x.OpeningReason, nullable: true)
                .Name("openingReason")
                .Description("Opening Reason");

            Field(x => x.PercentageOfPupilsReceivingFreeSchoolMeals, nullable: true)
                .Name("percentageOfPupilsReceivingFreeSchoolMeals")
                .Description("Percentage Of Pupils Receiving Free School Meals");

            Field(x => x.PhaseOfEducation, nullable: true)
                .Name("phaseOfEducation")
                .Description("Phase Of Education");

            Field(x => x.Postcode, nullable: true)
                .Name("postcode")
                .Description("Postcode");

            Field(x => x.PreviousEstablishmentNumber, nullable: true)
                .Name("previousEstablishmentNumber")
                .Description("Previous Establishment Number");

            Field(x => x.PreviousLocalAuthorityCode, nullable: true)
                .Name("previousLocalAuthorityCode")
                .Description("Previous Local Authority Code");

            Field(x => x.PreviousLocalAuthorityName, nullable: true)
                .Name("previousLocalAuthorityName")
                .Description("Previous Local Authority Name");

            Field(x => x.RscRegion, nullable: true)
                .Name("rscRegion")
                .Description("Rsc Region");

            Field(x => x.Section41Approved, nullable: true)
                .Name("section41Approved")
                .Description("Section41 Approved");

            Field<LearningProviderStatusEnum>(
                name: "Status",
                description: "Status",
                resolve: ctx => ctx.Source.Status);

            Field(x => x.StatutoryLowAge, nullable: true)
                .Name("statutoryLowAge")
                .Description("Statutory Low Age");

            Field(x => x.StatutoryHighAge, nullable: true)
                .Name("statutoryHighAge")
                .Description("Statutory High Age");

            Field<LearningProviderSubTypeEnum>(
                name: "SubType",
                description: "SubType",
                resolve: ctx => ctx.Source.SubType);

            Field(x => x.SixthForm, nullable: true)
                .Name("sixthForm")
                .Description("Sixth Form");

            Field(x => x.TeenMothers, nullable: true)
                .Name("teenMothers")
                .Description("Teen Mothers");

            Field(x => x.TeenMothersPlaces, nullable: true)
                .Name("teenMothersPlaces")
                .Description("Teen Mothers Places");

            Field<LearningProviderTypeEnum>(
                name: "Type",
                description: "Type",
                resolve: ctx => ctx.Source.Type);

            Field(x => x.UpdatedDate, nullable: true)
                .Name("updatedDate")
                .Description("Updated Date");

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

            Field(x => x.Website, nullable: true)
                .Name("website")
                .Description("Website");

            // Field(x => x.Name)
            //     .Name("name")
            //     .Description("Name of the learning provider");
            //
            // Field(x => x.LegalName, nullable: true)
            //     .Name("legalName")
            //     .Description("Legal name of the learning provider");
            //
            // Field(x => x.Urn, nullable: true)
            //     .Name("urn")
            //     .Description("URN of the learning provider");
            //
            // Field(x => x.Ukprn, nullable: true)
            //     .Name("ukprn")
            //     .Description("UKPRN of the learning provider");
            //
            // Field(x => x.Postcode, nullable: true)
            //     .Name("postcode")
            //     .Description("Postcode of the learning provider");
            //
            // Field(x => x.OpenDate, nullable: true)
            //     .Name("openDate")
            //     .Description("Date the learning provider opened");
            //
            // Field(x => x.CloseDate, nullable: true)
            //     .Name("closeDate")
            //     .Description("Date the learning provider closed");
            //
            //
            // Field(x => x.AcademyTrustCode, nullable: true)
            //     .Name("academyTrustCode")
            //     .Description("Academy trust code");
            //
            // Field(x => x.CharitiesCommissionNumber, nullable: true)
            //     .Name("charitiesCommissionNumber")
            //     .Description("Registered charity number");
            //
            // Field(x => x.CompaniesHouseNumber, nullable: true)
            //     .Name("companiesHouseNumber")
            //     .Description("Registered company number");
            //
            // Field(x => x.DfeNumber, nullable: true)
            //     .Name("dfeNumber")
            //     .Description("DfE Number");
            //
            // Field(x => x.LocalAuthorityCode, nullable: true)
            //     .Name("localAuthority")
            //     .Description("Local authority");
            //
            // Field(x => x.EstablishmentNumber, nullable: true)
            //     .Name("establishmentNumber")
            //     .Description("Establishment number");
            //
            // Field(x => x.PreviousEstablishmentNumber, nullable: true)
            //     .Name("previousEstablishmentNumber")
            //     .Description("Previous establishment number");
            //
            // Field(x => x.Uprn, nullable: true)
            //     .Name("uprn")
            //     .Description("UPRN");
            //
            //
            // Field<LearningProviderStatusEnum>(
            //     name: "Status",
            //     description: "Status of learning provider",
            //     resolve: ctx => ctx.Source.Status);
            //
            // Field<LearningProviderTypeEnum>(
            //     name: "Type",
            //     description: "Type of learning provider",
            //     resolve: ctx => ctx.Source.Type);
            //
            // Field<LearningProviderSubTypeEnum>(
            //     name: "SubType",
            //     description: "Sub-Type of learning provider",
            //     resolve: ctx => ctx.Source.SubType);
        }
    }
}