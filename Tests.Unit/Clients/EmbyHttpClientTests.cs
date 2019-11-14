using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Clients.Emby.Http.Model;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Net;
using FluentAssertions;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Moq;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serialization;
using Xunit;
using PluginInfo = MediaBrowser.Model.Plugins.PluginInfo;

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
            client.SetAddressAndUser("localhost:9000", "authtoken", "bf7edca9-8604-4dc7-8a18-403293939b90");

            var result = await client.GetInstalledPluginsAsync();
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0].Id.Should().Be(resultObj[0].Id);

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            usedRequest?.Parameters[0].Name.Should().Be("X-Emby-Authorization");
            usedRequest?.Parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\", Emby UserId=\"bf7edca9-8604-4dc7-8a18-403293939b90\"");
            usedRequest?.Parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            usedRequest?.Parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[1].Value.Should().Be("authtoken");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("Plugins");
        }

        [Fact]
        public async Task GetServerInfoAsync_Should_Return_Server_Info()
        {
            var resultObj = new ServerInfo { Id = Guid.NewGuid().ToString() };
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.SetAddressAndUser("localhost:9000", "authtoken", "bf7edca9-8604-4dc7-8a18-403293939b90");

            var result = await client.GetServerInfoAsync();
            result.Should().NotBeNull();
            result.Id.Should().Be(resultObj.Id);

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            usedRequest?.Parameters[0].Name.Should().Be("X-Emby-Authorization");
            usedRequest?.Parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\", Emby UserId=\"bf7edca9-8604-4dc7-8a18-403293939b90\"");
            usedRequest?.Parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            usedRequest?.Parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[1].Value.Should().Be("authtoken");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("System/Info");
        }

        [Fact]
        public async Task GetLocalDrivesAsync_Should_Return_List_Of_Drives()
        {
            var resultObj = new List<FileSystemEntryInfo>{ new FileSystemEntryInfo { Name = "Movies"}};
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.SetAddressAndUser("localhost:9000", "authtoken", "bf7edca9-8604-4dc7-8a18-403293939b90");

            var result = await client.GetLocalDrivesAsync();
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0].Name.Should().Be(resultObj[0].Name);

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            usedRequest?.Parameters[0].Name.Should().Be("X-Emby-Authorization");
            usedRequest?.Parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\", Emby UserId=\"bf7edca9-8604-4dc7-8a18-403293939b90\"");
            usedRequest?.Parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            usedRequest?.Parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[1].Value.Should().Be("authtoken");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("Environment/Drives");
        }

        [Fact]
        public async Task GetEmbyUsersAsync_Should_Return_List_Of_Users()
        {
            var resultObj = new JArray { new JObject { {"Id", "username"}}};
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.SetAddressAndUser("localhost:9000", "authtoken", "bf7edca9-8604-4dc7-8a18-403293939b90");

            var result = await client.GetEmbyUsersAsync();
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0]["Id"].Value<string>().Should().Be(resultObj[0]["Id"].Value<string>());

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            usedRequest?.Parameters[0].Name.Should().Be("X-Emby-Authorization");
            usedRequest?.Parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\", Emby UserId=\"bf7edca9-8604-4dc7-8a18-403293939b90\"");
            usedRequest?.Parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            usedRequest?.Parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[1].Value.Should().Be("authtoken");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("Users");
        }

        [Fact]
        public async Task GetEmbyDevicesAsync_Should_Return_List_Of_Devices()
        {
            var resultObj = new JObject { { "Id", Guid.NewGuid().ToString() } };
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.SetAddressAndUser("localhost:9000", "authtoken", "bf7edca9-8604-4dc7-8a18-403293939b90");

            var result = await client.GetEmbyDevicesAsync();
            result.Should().NotBeNull();
            result["Id"].Value<string>().Should().Be(resultObj["Id"].Value<string>());

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            usedRequest?.Parameters[0].Name.Should().Be("X-Emby-Authorization");
            usedRequest?.Parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\", Emby UserId=\"bf7edca9-8604-4dc7-8a18-403293939b90\"");
            usedRequest?.Parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            usedRequest?.Parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[1].Value.Should().Be("authtoken");

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
            client.SetAddressAndUser("localhost:9000", "authtoken", "bf7edca9-8604-4dc7-8a18-403293939b90");

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
            usedRequest?.Parameters[0].Name.Should().Be("SeriesStatuses");
            usedRequest?.Parameters[0].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[0].Value.Should().Be(string.Empty);
            usedRequest?.Parameters[1].Name.Should().Be("fields");
            usedRequest?.Parameters[1].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[1].Value.Should().Be(string.Empty);
            usedRequest?.Parameters[2].Name.Should().Be("Filters");
            usedRequest?.Parameters[2].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[2].Value.Should().Be(string.Empty);
            usedRequest?.Parameters[3].Name.Should().Be("ImageTypes");
            usedRequest?.Parameters[3].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[3].Value.Should().Be(string.Empty);
            usedRequest?.Parameters[4].Name.Should().Be("AirDays");
            usedRequest?.Parameters[4].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[4].Value.Should().Be(string.Empty);
            usedRequest?.Parameters[5].Name.Should().Be("EnableImageTypes");
            usedRequest?.Parameters[5].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[5].Value.Should().Be(string.Empty);
            usedRequest?.Parameters[6].Name.Should().Be("recursive");
            usedRequest?.Parameters[6].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[6].Value.Should().Be("False");
            usedRequest?.Parameters[7].Name.Should().Be("IncludeItemTypes");
            usedRequest?.Parameters[7].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[7].Value.Should().Be("Movies");
            usedRequest?.Parameters[8].Name.Should().Be("X-Emby-Authorization");
            usedRequest?.Parameters[8].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[8].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\", Emby UserId=\"bf7edca9-8604-4dc7-8a18-403293939b90\"");
            usedRequest?.Parameters[9].Name.Should().Be("X-MediaBrowser-Token");
            usedRequest?.Parameters[9].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[9].Value.Should().Be("authtoken");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("Items");
        }

        [Fact]
        public async Task GetPersonByNameAsync_Should_Return_Person()
        {
            var resultObj = new BaseItemDto {Id = Guid.NewGuid().ToString()};

            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");
            client.SetAddressAndUser("localhost:9000", "authtoken", "bf7edca9-8604-4dc7-8a18-403293939b90");

            var query = new ItemQuery
            {
                IncludeItemTypes = new[] { "Movies" }
            };

            var name = "test-person";
            var result = await client.GetPersonByNameAsync(name);
            result.Should().NotBeNull();
            result.Id.Should().Be(resultObj.Id);

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(9);
            usedRequest?.Parameters[0].Name.Should().Be("SeriesStatuses");
            usedRequest?.Parameters[0].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[0].Value.Should().Be(string.Empty);
            usedRequest?.Parameters[1].Name.Should().Be("fields");
            usedRequest?.Parameters[1].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[1].Value.Should().Be("PremiereDate");
            usedRequest?.Parameters[2].Name.Should().Be("Filters");
            usedRequest?.Parameters[2].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[2].Value.Should().Be(string.Empty);
            usedRequest?.Parameters[3].Name.Should().Be("ImageTypes");
            usedRequest?.Parameters[3].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[3].Value.Should().Be(string.Empty);
            usedRequest?.Parameters[4].Name.Should().Be("AirDays");
            usedRequest?.Parameters[4].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[4].Value.Should().Be(string.Empty);
            usedRequest?.Parameters[5].Name.Should().Be("EnableImageTypes");
            usedRequest?.Parameters[5].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[5].Value.Should().Be(string.Empty);
            usedRequest?.Parameters[6].Name.Should().Be("recursive");
            usedRequest?.Parameters[6].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[6].Value.Should().Be("False");
            usedRequest?.Parameters[7].Name.Should().Be("X-Emby-Authorization");
            usedRequest?.Parameters[7].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[7].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\", Emby UserId=\"bf7edca9-8604-4dc7-8a18-403293939b90\"");
            usedRequest?.Parameters[8].Name.Should().Be("X-MediaBrowser-Token");
            usedRequest?.Parameters[8].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[8].Value.Should().Be("authtoken");

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
            client.SetAddressAndUser("localhost:9000", "authtoken", "bf7edca9-8604-4dc7-8a18-403293939b90");

            var result = await client.GetMediaFoldersAsync();
            result.Should().NotBeNull();
            result.Items.Length.Should().Be(1);

            result.Items[0].Id.Should().Be(resultObj.Items[0].Id);

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(2);
            usedRequest?.Parameters[0].Name.Should().Be("X-Emby-Authorization");
            usedRequest?.Parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[0].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\", Emby UserId=\"bf7edca9-8604-4dc7-8a18-403293939b90\"");
            usedRequest?.Parameters[1].Name.Should().Be("X-MediaBrowser-Token");
            usedRequest?.Parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[1].Value.Should().Be("authtoken");

            usedRequest?.Method.Should().Be(Method.GET);
            usedRequest?.Resource.Should().Be("Library/MediaFolders");
        }

        [Fact]
        public void SetAddressAndUser_Should_Throw_Exception_If_Url_Is_Empty()
        {
            var resultObj = "test";
            var client = CreateClient(resultObj);
            Action act = () => client.SetAddressAndUser(string.Empty, "authtoken", "bf7edca9-8604-4dc7-8a18-403293939b90");
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetAddressAndUser_Should_Throw_Exception_If_Token_Is_Empty()
        {
            var resultObj = "test";
            var client = CreateClient(resultObj);
            Action act = () => client.SetAddressAndUser("localhost:9000", string.Empty, "bf7edca9-8604-4dc7-8a18-403293939b90");
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AuthenticateUserAsync_Should_Throw_Exception_If_UserName_Is_Empty()
        {
            var resultObj = "test";
            var client = CreateClient(resultObj);
            Action act = () => client.AuthenticateUserAsync(string.Empty, "pass", "localhsot:9000");
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AuthenticateUserAsync_Should_Throw_Exception_If_Pass_Is_Empty()
        {
            var resultObj = "test";
            var client = CreateClient(resultObj);
            Action act = () => client.AuthenticateUserAsync("username", string.Empty, "localhsot:9000");
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AuthenticateUserAsync_Should_Throw_Exception_If_Url_Is_Empty()
        {
            var resultObj = "test";
            var client = CreateClient(resultObj);
            Action act = () => client.AuthenticateUserAsync("username", "pass", string.Empty);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task AuthenticateUserAsync_Should_Return_Authentication_Info()
        {
            var resultObj = new AuthenticationResult()
            {
                AccessToken = "token",
                ServerId = Guid.NewGuid().ToString(),
                SessionInfo = new SessionInfoDto(),
                User = new UserDto()
            };

            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399");

            var result = await client.AuthenticateUserAsync("username+", "pass+", "localhost:9000");
            result.Should().NotBeNull();

            usedRequest.Should().NotBeNull();

            usedRequest?.Parameters.Count.Should().Be(3);
            usedRequest?.Parameters[0].Name.Should().Be("Username");
            usedRequest?.Parameters[0].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[0].Value.Should().Be("username+");
            usedRequest?.Parameters[1].Name.Should().Be("Pw");
            usedRequest?.Parameters[1].Type.Should().Be(ParameterType.QueryString);
            usedRequest?.Parameters[1].Value.Should().Be("pass+");
            usedRequest?.Parameters[2].Name.Should().Be("X-Emby-Authorization");
            usedRequest?.Parameters[2].Type.Should().Be(ParameterType.HttpHeader);
            usedRequest?.Parameters[2].Value.Should().Be("mediabrowser Client=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");


            usedRequest?.Method.Should().Be(Method.POST);
            usedRequest?.Resource.Should().Be("Users/AuthenticateByName");
        }
    }
}
