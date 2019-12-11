using System;
using AutoFixture.NUnit3;
using Dfe.Spi.GraphQlApi.Domain.Graph;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Dfe.Spi.GraphQlApi.Domain.UnitTests.Graph
{
    public class WhenParsingGraphRequest
    {
        private const string JsonMimeType = "application/json";
        private const string GraphQLMimeType = "application/graphql";
        
        [Test, AutoData]
        public void ThenItShouldReturnGraphRequestForJsonMimeType(GraphRequest request)
        {
            var value = JsonConvert.SerializeObject(request);
            
            var actual = GraphRequest.Parse(value, JsonMimeType);
            
            Assert.IsNotNull(actual);
        }
        
        [Test, AutoData]
        public void ThenItShouldDeserializeValueForJsonMimeType(GraphRequest request)
        {
            var value = JsonConvert.SerializeObject(request);
            
            var actual = GraphRequest.Parse(value, JsonMimeType);
            
            Assert.AreEqual(request.Query, actual.Query);
            Assert.AreEqual(request.OperationName, actual.OperationName);
            Assert.AreEqual(request.Variables.Count, actual.Variables.Count);
            foreach (var key in request.Variables.Keys)
            {
                Assert.IsTrue(actual.Variables.ContainsKey(key),
                    $"Expected Variables to contain key {key}, but was not found");
                Assert.AreEqual(request.Variables[key], actual.Variables[key],
                    $"Expected Variable[{key}] to be {request.Variables[key]}, but was {actual.Variables[key]}");
            }
        }
        
        [Test, AutoData]
        public void ThenItShouldReturnGraphRequestForJGraphQLMimeType(string value)
        {
            var actual = GraphRequest.Parse(value, GraphQLMimeType);
            
            Assert.IsNotNull(actual);
        }
        
        [Test, AutoData]
        public void ThenItShouldAssignValueToGraphRequestsQuery(string value)
        {
            var actual = GraphRequest.Parse(value, GraphQLMimeType);
            
            Assert.AreEqual(value, actual.Query);
        }

        [Test]
        public void ThenItShouldThrowExceptionIfMimeTypeNotSupported()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                GraphRequest.Parse("some-value", "text/plain"));
        }
    }
}