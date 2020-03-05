using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.UnitTesting.Fixtures;
using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.Models.Entities;
using GraphQL.Language.AST;
using GraphQL.Types;
using Moq;
using NUnit.Framework;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests.Resolvers
{
    public class WhenResolvingCensus
    {
        private Mock<IEntityRepository> _entityRepositoryMock;
        private Mock<ILoggerWrapper> _loggerMock;
        private CensusResolver _censusResolver;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Arrange()
        {
            _entityRepositoryMock = new Mock<IEntityRepository>();
            _entityRepositoryMock.Setup(r => r.LoadCensusAsync(
                    It.IsAny<LoadCensusRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityCollection<Census>
                {
                    SquashedEntityResults = new SquashedEntityResult<Census>[0],
                });

            _loggerMock = new Mock<ILoggerWrapper>();

            _censusResolver = new CensusResolver(
                _entityRepositoryMock.Object,
                _loggerMock.Object);

            _cancellationToken = new CancellationToken();
        }

        [Test]
        [NonRecursiveAutoData]
        public async Task ThenItShouldReturnSquashedEntity(Census census)
        {
            _entityRepositoryMock.Setup(r => r.LoadCensusAsync(
                    It.IsAny<LoadCensusRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityCollection<Census>
                {
                    SquashedEntityResults = new[]
                    {
                        new SquashedEntityResult<Census>
                        {
                            SquashedEntity = census,
                        },
                    },
                });
            var context = BuildLearningProviderResolveFieldContext();

            var actual = await _censusResolver.ResolveAsync(context);

            Assert.IsNotNull(actual);
            Assert.AreSame(census, actual);
            _entityRepositoryMock.Verify(r => r.LoadCensusAsync(
                    It.IsAny<LoadCensusRequest>(), It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Test]
        [NonRecursiveAutoData]
        public async Task ThenItShouldRequestCensusForYearTypeAndUrn(int year, string type, LearningProvider source)
        {
            var context = BuildLearningProviderResolveFieldContext(source, year, type);

            await _censusResolver.ResolveAsync(context);

            var expectedId = $"{year}-{type}-{nameof(LearningProvider)}-{source.Urn}";
            _entityRepositoryMock.Verify(r => r.LoadCensusAsync(
                    It.Is<LoadCensusRequest>(req =>
                        req.EntityReferences.Length == 1 &&
                        req.EntityReferences[0].AdapterRecordReferences.Length == 1 &&
                        req.EntityReferences[0].AdapterRecordReferences[0].SourceSystemName ==
                        SourceSystemNames.IStore &&
                        req.EntityReferences[0].AdapterRecordReferences[0].SourceSystemId == expectedId),
                    context.CancellationToken),
                Times.Once());
        }

        [Test]
        [AutoData]
        public async Task ThenItShouldRequestCensusAggregates(AggregationRequest aggregationRequest1, AggregationRequest aggregationRequest2)
        {
            var context = BuildLearningProviderResolveFieldContext(
                aggregationRequests: new[] { aggregationRequest1, aggregationRequest2});

            await _censusResolver.ResolveAsync(context);
            
            _entityRepositoryMock.Verify(r => r.LoadCensusAsync(
                    It.Is<LoadCensusRequest>(req =>
                        req.Aggregations != null &&
                        req.Aggregations.Length == 2 &&
                        req.Aggregations[0].Name == aggregationRequest1.Name &&
                        req.Aggregations[0].Conditions.Length == aggregationRequest1.Conditions.Length &&
                        req.Aggregations[1].Name == aggregationRequest2.Name &&
                        req.Aggregations[1].Conditions.Length == aggregationRequest2.Conditions.Length),
                    context.CancellationToken),
                Times.Once());
        }


        private ResolveFieldContext<LearningProvider> BuildLearningProviderResolveFieldContext(
            LearningProvider source = null, int year = 2020, string type = "SchoolSummer",
            string[] fields = null, AggregationRequest[] aggregationRequests = null)
        {
            if (source == null)
            {
                source = new LearningProvider();
            }

            if (fields == null)
            {
                fields = new[] {"name"};
            }

            if (aggregationRequests != null && !fields.Any(f => f == "_aggregations"))
            {
                fields = fields.Concat(new[] {"_aggregations"}).ToArray();
            }

            var context = TestHelper.BuildResolveFieldContext(
                source: source,
                arguments: new Dictionary<string, object>
                {
                    {"year", year},
                    {"type", type},
                },
                fields: fields);
            
            if (aggregationRequests != null)
            {
                var aggregationsField = context.FieldAst.SelectionSet.Selections
                    .Select(x => (Field) x)
                    .Single(f => f.Name == "_aggregations");
                aggregationsField.Arguments = new Arguments
                {
                    new Argument(new NameNode("definitions"))
                    {
                        Value = new ListValue(aggregationRequests.Select(request =>
                            new ObjectValue(new[]
                            {
                                new ObjectField("name", new StringValue(request.Name)),
                                new ObjectField("conditions", 
                                    new ListValue(request.Conditions.Select(condition =>
                                        new ObjectValue(new[]
                                        {
                                            new ObjectField("field", new StringValue(condition.Field)), 
                                            new ObjectField("operator", new StringValue(condition.Operator.ToString().ToUpper())), 
                                            new ObjectField("value", new StringValue(condition.Value)), 
                                        })))), 
                            }))),
                    }
                };
            }

            return context;
        }
    }
}