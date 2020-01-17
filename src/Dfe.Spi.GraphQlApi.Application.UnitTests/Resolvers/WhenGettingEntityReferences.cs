using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Moq;
using NUnit.Framework;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests.Resolvers
{
    public class WhenGettingEntityReferences
    {
        private Mock<Func<SearchRequest, CancellationToken, Task<SearchResultSet<LearningProviderReference>>>>
            _searchFuncMock;
        private Mock<Func<string, string, CancellationToken, Task<EntityReference[]>>> _getSynonymsFuncMock;
        private EntityReferenceBuilder<LearningProviderReference> _builder;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Arrange()
        {
            _searchFuncMock =
                new Mock<Func<SearchRequest, CancellationToken, Task<SearchResultSet<LearningProviderReference>>>>();
            _searchFuncMock.Setup(f =>
                    f.Invoke(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResultSet<LearningProviderReference>
                {
                    Documents = new LearningProviderReference[0],
                });
            
            _getSynonymsFuncMock = new Mock<Func<string, string, CancellationToken, Task<EntityReference[]>>>();
            _getSynonymsFuncMock.Setup(f =>
                    f.Invoke(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityReference[0]);

            _builder = new EntityReferenceBuilder<LearningProviderReference>(
                _searchFuncMock.Object,
                _getSynonymsFuncMock.Object);

            _cancellationToken = new CancellationToken();
        }

        [Test, AutoData]
        public async Task ThenItShouldSearchUsingProvidedRequest(SearchRequest request)
        {
            await _builder.GetEntityReferences(request, _cancellationToken);

            _searchFuncMock.Verify(f =>
                    f(request, _cancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldGetSynonymsForEachSearchResult(
            SearchResultSet<LearningProviderReference> searchResults)
        {
            _searchFuncMock.Setup(f =>
                    f.Invoke(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(searchResults);
            
            await _builder.GetEntityReferences(new SearchRequest(), _cancellationToken);
            
            _getSynonymsFuncMock.Verify(f =>
                f.Invoke(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Exactly(searchResults.Documents.Length));
            for (var i = 0; i < searchResults.Documents.Length; i++)
            {
                var result = searchResults.Documents[i];
                _getSynonymsFuncMock.Verify(f =>
                        f.Invoke(result.SourceSystemName, result.SourceSystemId, _cancellationToken),
                    Times.Once,
                    $"Expected _getSynonymsFunc to be called for result {i} but it was not " +
                    $"(SourceSystemName: {result.SourceSystemName}, SourceSystemId: {result.SourceSystemId})");
            }
        }

        [Test, AutoData]
        public async Task ThenItShouldReturnReferencePerResultWithSynonyms(
            SearchResultSet<LearningProviderReference> searchResults)
        {
            _searchFuncMock.Setup(f =>
                    f.Invoke(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(searchResults);
            foreach (var result in searchResults.Documents)
            {
                _getSynonymsFuncMock.Setup(f =>
                        f.Invoke(result.SourceSystemName, result.SourceSystemId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new[]
                    {
                        new EntityReference
                        {
                            SourceSystemName = result.SourceSystemName + "-Syn",
                            SourceSystemId = result.SourceSystemId + "-Syn",
                        },
                    });
            }
            
            var actual = await _builder.GetEntityReferences(new SearchRequest(), _cancellationToken);
            
            Assert.IsNotNull(actual);
            Assert.AreEqual(searchResults.Documents.Length, actual.Length);
            for (var i = 0; i < searchResults.Documents.Length; i++)
            {
                var result = searchResults.Documents[i];
                var reference = actual[i];
                
                Assert.IsNotNull(reference);
                Assert.AreEqual(2, reference.AdapterRecordReferences.Length);
                Assert.AreEqual(result.SourceSystemName, reference.AdapterRecordReferences[0].SourceSystemName);
                Assert.AreEqual(result.SourceSystemId, reference.AdapterRecordReferences[0].SourceSystemId);
                Assert.AreEqual(result.SourceSystemName + "-Syn", reference.AdapterRecordReferences[1].SourceSystemName);
                Assert.AreEqual(result.SourceSystemId + "-Syn", reference.AdapterRecordReferences[1].SourceSystemId);
            }
        }
    }
}