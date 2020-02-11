using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.UnitTesting.Fixtures;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Dfe.Spi.Models;
using GraphQL.Types;
using Moq;
using NUnit.Framework;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests.Resolvers
{
    public class WhenResolvingLearningProvider
    {
        private Mock<IEntityRepository> _entityRepositoryMock;
        private Mock<ILoggerWrapper> _loggerMock;
        private Mock<IEntityReferenceBuilder> _entityReferenceBuilderMock;
        private LearningProviderResolver _resolver;

        [SetUp]
        public void Arrange()
        {
            _entityRepositoryMock = new Mock<IEntityRepository>();
            _entityRepositoryMock.Setup(r =>
                    r.LoadLearningProvidersAsync(It.IsAny<LoadLearningProvidersRequest>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityCollection<LearningProvider>
                {
                    SquashedEntityResults = new SquashedEntityResult<LearningProvider>[0],
                });

            _loggerMock = new Mock<ILoggerWrapper>();

            _entityReferenceBuilderMock = new Mock<IEntityReferenceBuilder>();
            _entityReferenceBuilderMock.Setup(b =>
                    b.GetEntityReferences(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] {new AggregateEntityReference()});

            _resolver = new LearningProviderResolver(
                _entityRepositoryMock.Object,
                _loggerMock.Object,
                _entityReferenceBuilderMock.Object);
        }

        [Test, AutoData]
        public async Task ThenItShouldGetEntitiesReferencesFromBuilderUsingUrnFilter(long urn)
        {
            var context = BuildResolveFieldContext(urn.ToString());

            await _resolver.ResolveAsync(context);

            _entityReferenceBuilderMock.Verify(b => b.GetEntityReferences(
                    It.Is<SearchRequest>(r =>
                        r.Filter != null &&
                        r.Filter.Length == 1 &&
                        r.Filter[0].Field == "urn" &&
                        r.Filter[0].Value == urn.ToString()),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldGetReferencedEntitiesFromRepository(AggregateEntityReference reference)
        {
            _entityReferenceBuilderMock.Setup(b =>
                    b.GetEntityReferences(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] {reference});
            var context = BuildResolveFieldContext();

            await _resolver.ResolveAsync(context);

            _entityRepositoryMock.Verify(er => er.LoadLearningProvidersAsync(
                    It.Is<LoadLearningProvidersRequest>(r =>
                        r.EntityName == "LearningProvider" &&
                        r.EntityReferences != null &&
                        r.EntityReferences.Length == 1 &&
                        r.EntityReferences[0] == reference),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldUseRequestedFieldsWhenGettingReferencedEntities(
            AggregateEntityReference reference)
        {
            _entityReferenceBuilderMock.Setup(b =>
                    b.GetEntityReferences(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] {reference});
            var context = BuildResolveFieldContext(fields: new[]
            {
                "urn",
                "ukprn",
                "name"
            });

            await _resolver.ResolveAsync(context);

            _entityRepositoryMock.Verify(er => er.LoadLearningProvidersAsync(
                    It.Is<LoadLearningProvidersRequest>(r =>
                        r.Fields != null &&
                        r.Fields.Length == 3 &&
                        r.Fields[0] == "urn" &&
                        r.Fields[1] == "ukprn" &&
                        r.Fields[2] == "name"),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, NonRecursiveAutoData]
        public async Task ThenItShouldReturnEntitiesFromRepository(LearningProvider learningProvider)
        {
            _entityRepositoryMock.Setup(r =>
                    r.LoadLearningProvidersAsync(It.IsAny<LoadLearningProvidersRequest>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityCollection<LearningProvider>
                {
                    SquashedEntityResults = new[]
                    {
                        new SquashedEntityResult<LearningProvider>
                        {
                            SquashedEntity = learningProvider
                        },
                    },
                });
            var context = BuildResolveFieldContext();

            var actual = await _resolver.ResolveAsync(context);

            Assert.IsNotNull(actual);
            Assert.AreEqual(learningProvider.Urn, actual.Urn);
            Assert.AreEqual(learningProvider.Ukprn, actual.Ukprn);
            Assert.AreEqual(learningProvider.Name, actual.Name);
        }

        [Test]
        public async Task ThenItShouldReturnNullIfNoEntityReferenceFound()
        {
            _entityReferenceBuilderMock.Setup(b =>
                    b.GetEntityReferences(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AggregateEntityReference[0]);
            var context = BuildResolveFieldContext();

            var actual = await _resolver.ResolveAsync(context);

            Assert.IsNull(actual);
        }

        [Test]
        public void ThenItShouldThrowExceptionIfBuilderThrowsException()
        {
            var ex = new Exception("Unit test error");
            _entityReferenceBuilderMock.Setup(b =>
                    b.GetEntityReferences(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(ex);
            var context = BuildResolveFieldContext();

            var actual = Assert.ThrowsAsync<Exception>(async () =>
                await _resolver.ResolveAsync(context));
            Assert.AreSame(ex, actual);
        }

        [Test]
        public async Task ThenItShouldSetErrorOnContextIfNoArgumentsProvided()
        {
            var context = TestHelper.BuildResolveFieldContext<object>(arguments: new Dictionary<string, object>());

            await _resolver.ResolveAsync(context);

            Assert.IsNotNull(context.Errors);
            Assert.AreEqual(1, context.Errors.Count);
            Assert.AreEqual("Must provide one argument", context.Errors[0].Message);
        }

        [Test]
        public async Task ThenItShouldSetErrorOnContextIfMoreThanOneArgumentsProvided()
        {
            var context = TestHelper.BuildResolveFieldContext<object>(arguments: new Dictionary<string, object>
            {
                {"urn", "12345678"},
                {"dfeNumber", "123/4567"},
            });

            await _resolver.ResolveAsync(context);

            Assert.IsNotNull(context.Errors);
            Assert.AreEqual(1, context.Errors.Count);
            Assert.AreEqual("Must provide one argument", context.Errors[0].Message);
        }

        [Test]
        public void ThenItShouldThrowExceptionIfRepositoryThrowException()
        {
            var ex = new Exception("Unit test error");
            _entityRepositoryMock.Setup(r =>
                    r.LoadLearningProvidersAsync(It.IsAny<LoadLearningProvidersRequest>(),
                        It.IsAny<CancellationToken>()))
                .ThrowsAsync(ex);
            var context = BuildResolveFieldContext();

            var actual = Assert.ThrowsAsync<Exception>(async () =>
                await _resolver.ResolveAsync(context));
            Assert.AreSame(ex, actual);
        }


        private ResolveFieldContext<object> BuildResolveFieldContext(string urn = null, string[] fields = null)
        {
            return TestHelper.BuildResolveFieldContext<object>(arguments: new Dictionary<string, object>
            {
                {"urn", urn ?? TestHelper.RandomInt(10000000, 99999999).ToString()},
            }, fields: fields);
        }
    }
}