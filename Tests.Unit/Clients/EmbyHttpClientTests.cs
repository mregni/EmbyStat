using System;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RestSharp;
using RestSharp.Serialization;
using Xunit;

namespace Tests.Unit.Clients
{
    public class EmbyHttpClientTests
    {
        private Mock<IRestClient> _restClientMock;
        private IRestRequest _usedRequest;
        private EmbyHttpClient CreateClient<T>(T returnObject) where T : new()
        {
            var response = new RestResponse<T> { Data = returnObject };
            
            _restClientMock = new Mock<IRestClient>();
            _restClientMock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                .Callback<IRestRequest>((request) =>
                {
                    _usedRequest = request;
                })
                .Returns(response);
            _restClientMock.Setup(x => x.UseSerializer(It.IsAny<JsonNetSerializer>));
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            return new EmbyHttpClient(_restClientMock.Object, httpContextAccessorMock.Object);
        }

        private EmbyHttpClient CreateStringClient(string returnObject)
        {
            var response = new RestResponse { Content = returnObject };

            _restClientMock = new Mock<IRestClient>();
            _restClientMock.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Callback<IRestRequest>((request) =>
                {
                    _usedRequest = request;
                })
                .Returns(response);
            _restClientMock.Setup(x => x.UseSerializer(It.IsAny<JsonNetSerializer>));
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            return new EmbyHttpClient(_restClientMock.Object, httpContextAccessorMock.Object);
        }

        [Fact]
        public void PingEmby_Should_Return_Emby_String()
        {
            var client = CreateStringClient("Emby Server");
            var result = client.Ping();

            result.Should().Be(true);
        }

        [Fact]
        public void PingEmby_Should_Return_False()
        {
            _restClientMock = new Mock<IRestClient>();
            _restClientMock.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Throws<Exception>();
            _restClientMock.Setup(x => x.UseSerializer(It.IsAny<IRestSerializer>)).Returns(_restClientMock.Object);
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var client = new EmbyHttpClient(_restClientMock.Object, httpContextAccessorMock.Object);
            var result = client.Ping();

            result.Should().Be(false);
        }
    }
}
