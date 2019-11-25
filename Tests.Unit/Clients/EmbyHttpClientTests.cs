using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Clients.Emby.Http.Model;
using EmbyStat.Common.Models.Entities;
using FluentAssertions;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Moq;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serialization;
using Xunit;

namespace Tests.Unit.Clients
{
    public class EmbyHttpClientTests
    {
        private Mock<IRestClient> _restClientMock;
        private IRestRequest usedRequest;
        private EmbyClient CreateClient<T>(T returnObject)
        {
            var response = new RestResponse<T> { Data = returnObject };
            
            _restClientMock = new Mock<IRestClient>();
            _restClientMock.Setup(x => x.ExecuteTaskAsync<T>(It.IsAny<IRestRequest>()))
                .Callback<IRestRequest>((request) =>
                {
                    usedRequest = request;
                })
                .ReturnsAsync(response);
            _restClientMock.Setup(x => x.UseSerializer(It.IsAny<IRestSerializer>())).Returns(_restClientMock.Object);

            return new EmbyClient(_restClientMock.Object);
        }

        [Fact]
        public async Task GetInstalledPluginsAsync_Should_Return_List_Of_Plugins()
        {
            var resultObj = new List<PluginInfo> { new PluginInfo { Id = Guid.NewGuid().ToString() } };
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = await client.GetInstalledPluginsAsync();
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0].Id.Should().Be(resultObj[0].Id);

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("Plugins");
        }

        [Fact]
        public async Task GetServerInfoAsync_Should_Return_Server_Info()
        {
            var resultObj = new ServerInfo { Id = Guid.NewGuid().ToString() };
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = await client.GetServerInfoAsync();
            result.Should().NotBeNull();
            result.Id.Should().Be(resultObj.Id);

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("System/Info");
        }

        [Fact]
        public async Task GetLocalDrivesAsync_Should_Return_List_Of_Drives()
        {
            var resultObj = new List<FileSystemEntryInfo>{ new FileSystemEntryInfo { Name = "Movies"}};
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = await client.GetLocalDrivesAsync();
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0].Name.Should().Be(resultObj[0].Name);

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("Environment/Drives");
        }

        [Fact]
        public async Task GetEmbyUsersAsync_Should_Return_List_Of_Users()
        {
            var resultObj = new JArray { new JObject { {"Id", "username"}}};
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = await client.GetEmbyUsersAsync();
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0]["Id"].Value<string>().Should().Be(resultObj[0]["Id"].Value<string>());

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("Users");
        }

        [Fact]
        public async Task GetEmbyDevicesAsync_Should_Return_List_Of_Devices()
        {
            var resultObj = new JObject { { "Id", Guid.NewGuid().ToString() } };
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = await client.GetEmbyDevicesAsync();
            result.Should().NotBeNull();
            result["Id"].Value<string>().Should().Be(resultObj["Id"].Value<string>());

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("Devices");
        }

        [Fact]
        public async Task GetItemsAsync_Should_Return_List_Of_Items()
        {
            var resultObj = new QueryResult<BaseItemDto>
            {
                Items = new[]
                {
                    new BaseItemDto {Id = Guid.NewGuid().ToString()}
                }
            };

            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var query = new ItemQuery
            {
                IncludeItemTypes = new[] {"Movies"}
            };

            var result = await client.GetItemsAsync(query);
            result.Should().NotBeNull();
            result.Items.Length.Should().Be(1);

            result.Items[0].Id.Should().Be(resultObj.Items[0].Id);

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(10);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("AirDays");
            parameters[0].Type.Should().Be(ParameterType.QueryString);
            parameters[0].Value.Should().Be(string.Empty);
            parameters[1].Name.Should().Be("EnableImageTypes");
            parameters[1].Type.Should().Be(ParameterType.QueryString);
            parameters[1].Value.Should().Be(string.Empty);
            parameters[2].Name.Should().Be("fields");
            parameters[2].Type.Should().Be(ParameterType.QueryString);
            parameters[2].Value.Should().Be(string.Empty);
            parameters[3].Name.Should().Be("Filters");
            parameters[3].Type.Should().Be(ParameterType.QueryString);
            parameters[3].Value.Should().Be(string.Empty);
            parameters[4].Name.Should().Be("ImageTypes");
            parameters[4].Type.Should().Be(ParameterType.QueryString);
            parameters[4].Value.Should().Be(string.Empty);
            parameters[5].Name.Should().Be("IncludeItemTypes");
            parameters[5].Type.Should().Be(ParameterType.QueryString);
            parameters[5].Value.Should().Be("Movies");
            parameters[6].Name.Should().Be("recursive");
            parameters[6].Type.Should().Be(ParameterType.QueryString);
            parameters[6].Value.Should().Be("False");
            parameters[7].Name.Should().Be("SeriesStatuses");
            parameters[7].Type.Should().Be(ParameterType.QueryString);
            parameters[7].Value.Should().Be(string.Empty);
            parameters[8].Name.Should().Be("X-Emby-Authorization");
            parameters[8].Type.Should().Be(ParameterType.HttpHeader);
            parameters[8].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[9].Name.Should().Be("X-MediaBrowser-Token");
            parameters[9].Type.Should().Be(ParameterType.HttpHeader);
            parameters[9].Value.Should().Be("apikey");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("Items");
        }

        [Fact]
        public async Task GetPersonByNameAsync_Should_Return_Person()
        {
            var resultObj = new BaseItemDto {Id = Guid.NewGuid().ToString()};

            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var name = "test-person";
            var result = await client.GetPersonByNameAsync(name);
            result.Should().NotBeNull();
            result.Id.Should().Be(resultObj.Id);

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(9);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("AirDays");
            parameters[0].Type.Should().Be(ParameterType.QueryString);
            parameters[0].Value.Should().Be(string.Empty);
            parameters[1].Name.Should().Be("EnableImageTypes");
            parameters[1].Type.Should().Be(ParameterType.QueryString);
            parameters[1].Value.Should().Be(string.Empty);
            parameters[2].Name.Should().Be("fields");
            parameters[2].Type.Should().Be(ParameterType.QueryString);
            parameters[2].Value.Should().Be("PremiereDate");
            parameters[3].Name.Should().Be("Filters");
            parameters[3].Type.Should().Be(ParameterType.QueryString);
            parameters[3].Value.Should().Be(string.Empty);
            parameters[4].Name.Should().Be("ImageTypes");
            parameters[4].Type.Should().Be(ParameterType.QueryString);
            parameters[4].Value.Should().Be(string.Empty);
            parameters[5].Name.Should().Be("recursive");
            parameters[5].Type.Should().Be(ParameterType.QueryString);
            parameters[5].Value.Should().Be("False");
            parameters[6].Name.Should().Be("SeriesStatuses");
            parameters[6].Type.Should().Be(ParameterType.QueryString);
            parameters[6].Value.Should().Be(string.Empty);
            parameters[7].Name.Should().Be("X-Emby-Authorization");
            parameters[7].Type.Should().Be(ParameterType.HttpHeader);
            parameters[7].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[8].Name.Should().Be("X-MediaBrowser-Token");
            parameters[8].Type.Should().Be(ParameterType.HttpHeader);
            parameters[8].Value.Should().Be("apikey");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be($"persons/{name}");
        }

        [Fact]
        public async Task GetMediaFoldersAsync_Should_Return_List_Of_Media_Folders()
        {
            var resultObj = new QueryResult<BaseItemDto>
            {
                Items = new[]
                {
                    new BaseItemDto {Id = Guid.NewGuid().ToString()}
                }
            };

            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = await client.GetMediaFoldersAsync();
            result.Should().NotBeNull();
            result.Items.Length.Should().Be(1);

            result.Items[0].Id.Should().Be(resultObj.Items[0].Id);

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("Library/MediaFolders");
        }

        [Fact]
        public void SetAddressAndUser_Should_Throw_Exception_If_Url_Is_Empty()
        {
            var resultObj = "test";
            var client = CreateClient(resultObj);
            Action act = () => client.BaseUrl = string.Empty;
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetAddressAndUser_Should_Throw_Exception_If_ApiKey_Is_Empty()
        {
            var resultObj = "test";
            var client = CreateClient(resultObj);
            Action act = () => client.ApiKey = string.Empty;
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task PingEmbyAsync_Should_Return_Emby_String()
        {
            var client = CreateClient("Emby Server");
            var result = await client.PingEmbyAsync();

            result.Should().Be("Emby Server");
        }

        [Fact]
        public async Task PingEmbyAsync_Should_Return_Failed_String()
        {
            _restClientMock = new Mock<IRestClient>();
            _restClientMock.Setup(x => x.ExecuteTaskAsync<string>(It.IsAny<IRestRequest>()))
                .Throws<Exception>();
            _restClientMock.Setup(x => x.UseSerializer(It.IsAny<IRestSerializer>())).Returns(_restClientMock.Object);

            var client = new EmbyClient(_restClientMock.Object);
            var result = await client.PingEmbyAsync();

            result.Should().Be("Ping failed");
        }
    }
}
