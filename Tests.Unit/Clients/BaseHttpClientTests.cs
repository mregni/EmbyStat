using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.Net;
using FluentAssertions;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json.Linq;
using RestSharp;
using Tests.Unit.Builders;
using Tests.Unit.Helpers;
using Xunit;

namespace Tests.Unit.Clients
{
    public class BaseHttpClientTests
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
        public void SetAddressAndUser_Should_Throw_Exception_If_Url_Is_Empty()
        {
            var resultObj = "test";
            var client = CreateStringClient(resultObj);
            client.BaseUrl = string.Empty;

            client.BaseUrl.Should().BeEmpty();
        }

        [Fact]
        public void SetAddressAndUser_Should_Throw_Exception_If_ApiKey_Is_Empty()
        {
            var resultObj = "test";
            var client = CreateStringClient(resultObj);
            client.ApiKey = string.Empty;

            client.ApiKey.Should().BeEmpty();
        }

        [Fact]
        public void GetPersonByName_Should_Return_Person()
        {
            var resultObj = new BaseItemDto { Id = Guid.NewGuid().ToString() };

            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var name = "test-person";
            var result = client.GetPersonByName(name);
            result.Should().NotBeNull();
            result.Id.Should().Be(resultObj.Id);

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(10);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters.CheckQueryParameter(0, "AirDays", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(1, "EnableImageTypes", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(2, "fields", "PremiereDate", ParameterType.QueryString);
            parameters.CheckQueryParameter(3, "Filters", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(4, "ImageTypes", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(5, "recursive", "False", ParameterType.QueryString);
            parameters.CheckQueryParameter(6, "SeriesStatuses", "", ParameterType.QueryString);
            parameters.CheckQueryParameter(7, "UserId", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246", ParameterType.QueryString);
            parameters.CheckQueryParameter(8, "X-Emby-Authorization", "mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"", ParameterType.HttpHeader);
            parameters.CheckQueryParameter(9, "X-Emby-Token", "apikey", ParameterType.HttpHeader);

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be($"persons/{name}");
        }

        [Fact]
        public void GetPersonByName_Should_Return_Null_If_No_Response()
        {
            var client = CreateClient((BaseItemDto)null);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var name = "test-person";
            var result = client.GetPersonByName(name);
            result.Should().BeNull();

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(10);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters.CheckQueryParameter(0, "AirDays", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(1, "EnableImageTypes", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(2, "fields", "PremiereDate", ParameterType.QueryString);
            parameters.CheckQueryParameter(3, "Filters", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(4, "ImageTypes", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(5, "recursive", "False", ParameterType.QueryString);
            parameters.CheckQueryParameter(6, "SeriesStatuses", "", ParameterType.QueryString);
            parameters.CheckQueryParameter(7, "UserId", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246", ParameterType.QueryString);
            parameters.CheckQueryParameter(8, "X-Emby-Authorization", "mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"", ParameterType.HttpHeader);
            parameters.CheckQueryParameter(9, "X-Emby-Token", "apikey", ParameterType.HttpHeader);

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be($"persons/{name}");
        }

        [Fact]
        public void GetMediaFolders_Should_Return_List_Of_Media_Folders()
        {
            var resultObj = new QueryResult<BaseItemDto>
            {
                Items = new[]
                {
                    new BaseItemDto {Id = Guid.NewGuid().ToString()}
                }
            };

            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = client.GetMediaFolders();
            result.Should().NotBeNull();
            result.Items.Length.Should().Be(1);

            result.Items[0].Id.Should().Be(resultObj.Items[0].Id);

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-Emby-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be("Library/MediaFolders");
        }

        [Fact]
        public void GetInstalledPlugins_Should_Return_List_Of_Plugins()
        {
            var resultObj = new List<PluginInfo> { new PluginInfo { Id = Guid.NewGuid().ToString() } };
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = client.GetInstalledPlugins();
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0].Id.Should().Be(resultObj[0].Id);

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-Emby-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be("Plugins");
        }

        [Fact]
        public void GetServerInfo_Should_Return_Server_Info()
        {
            var resultObj = new ServerInfoDto { Id = Guid.NewGuid().ToString() };
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = client.GetServerInfo();
            result.Should().NotBeNull();
            result.Id.Should().Be(resultObj.Id);

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-Emby-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be("System/Info");
        }

        [Fact]
        public void GetLocalDrives_Should_Return_List_Of_Drives()
        {
            var resultObj = new List<FileSystemEntryInfo> { new FileSystemEntryInfo { Name = "Movies" } };
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = client.GetLocalDrives();
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0].Name.Should().Be(resultObj[0].Name);

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-Emby-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be("Environment/Drives");
        }

        [Fact]
        public void GetUsers_Should_Return_List_Of_Users()
        {
            var resultObj = new JArray { new JObject { { "Id", "username" } } };
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = client.GetUsers();
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0]["Id"].Value<string>().Should().Be(resultObj[0]["Id"].Value<string>());

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-Emby-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be("Users");
        }

        [Fact]
        public void GetDevices_Should_Return_List_Of_Devices()
        {
            var resultObj = new JObject { { "Id", Guid.NewGuid().ToString() } };
            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = client.GetDevices();
            result.Should().NotBeNull();
            result["Id"].Value<string>().Should().Be(resultObj["Id"].Value<string>());

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(2);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();
            parameters[0].Name.Should().Be("X-Emby-Authorization");
            parameters[0].Type.Should().Be(ParameterType.HttpHeader);
            parameters[0].Value.Should().Be("mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"");
            parameters[1].Name.Should().Be("X-Emby-Token");
            parameters[1].Type.Should().Be(ParameterType.HttpHeader);
            parameters[1].Value.Should().Be("apikey");

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be("Devices");
        }

        [Fact]
        public void GetMovies_Should_Return_List_Of_Movies()
        {
            var resultObj = new QueryResult<BaseItemDto>
            {
                Items = new[]
                {
                    new MovieBuilder("123").BuildBaseItemDto()
                }
            };

            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = client.GetMovies("123", "0", 1000, 100, DateTime.MinValue);
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0].Id.Should().Be(resultObj.Items[0].Id);

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(16);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();

            parameters.CheckQueryParameter(0, "AirDays", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(1, "EnableImages", "True", ParameterType.QueryString);
            parameters.CheckQueryParameter(2, "EnableImageTypes", "Banner,Primary,Thumb,Logo", ParameterType.QueryString);
            parameters.CheckQueryParameter(3, "fields", "Genres,DateCreated,MediaSources,ExternalUrls,OriginalTitle,Studios,MediaStreams,Path,Overview,ProviderIds,SortName,ParentId,People,PremiereDate,CommunityRating,OfficialRating,ProductionYear,RunTimeTicks", ParameterType.QueryString);
            parameters.CheckQueryParameter(4, "Filters", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(5, "ImageTypes", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(6, "IncludeItemTypes", "Movie", ParameterType.QueryString);
            parameters.CheckQueryParameter(7, "Limit", "100", ParameterType.QueryString);
            parameters.CheckQueryParameter(8, "LocationTypes", "FileSystem", ParameterType.QueryString);
            parameters.CheckQueryParameter(9, "ParentId", "123", ParameterType.QueryString);
            parameters.CheckQueryParameter(10, "recursive", "True", ParameterType.QueryString);
            parameters.CheckQueryParameter(11, "SeriesStatuses", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(12, "StartIndex", "1000", ParameterType.QueryString);
            parameters.CheckQueryParameter(13, "UserId", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246", ParameterType.QueryString);
            parameters.CheckQueryParameter(14, "X-Emby-Authorization", "mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"", ParameterType.HttpHeader);
            parameters.CheckQueryParameter(15, "X-Emby-Token", "apikey", ParameterType.HttpHeader);

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be("Items");
        }

        [Fact]
        public void GetShows_Should_Return_List_Of_Shows()
        {
            var resultObj = new QueryResult<BaseItemDto>
            {
                Items = new[]
                {
                    new ShowBuilder("12", "23").BuildBaseItemDto() 
                }
            };

            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = client.GetShows("123", DateTime.MinValue);
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0].Id.Should().Be(resultObj.Items[0].Id);

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(15);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();

            parameters.CheckQueryParameter(0, "AirDays", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(1, "EnableImageTypes", "Banner,Primary,Thumb,Logo", ParameterType.QueryString);
            parameters.CheckQueryParameter(2, "fields", "OriginalTitle,Genres,DateCreated,ExternalUrls,Studios,Path,ProviderIds,SortName,ParentId,People,PremiereDate,CommunityRating,OfficialRating,ProductionYear,Status,RunTimeTicks", ParameterType.QueryString);
            parameters.CheckQueryParameter(3, "Filters", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(4, "ImageTypes", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(5, "IncludeItemTypes", "Series", ParameterType.QueryString);
            parameters.CheckQueryParameter(6, "LocationTypes", "FileSystem", ParameterType.QueryString);
            parameters.CheckQueryParameter(7, "ParentId", "123", ParameterType.QueryString);
            parameters.CheckQueryParameter(8, "recursive", "True", ParameterType.QueryString);
            parameters.CheckQueryParameter(9, "SeriesStatuses", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(10, "SortBy", "SortName", ParameterType.QueryString);
            parameters.CheckQueryParameter(11, "sortOrder", "Ascending", ParameterType.QueryString);
            parameters.CheckQueryParameter(12, "UserId", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246", ParameterType.QueryString);
            parameters.CheckQueryParameter(13, "X-Emby-Authorization", "mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"", ParameterType.HttpHeader);
            parameters.CheckQueryParameter(14, "X-Emby-Token", "apikey", ParameterType.HttpHeader);

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be("Items");
        }

        [Fact]
        public void GetSeasons_Should_Return_List_Of_Season()
        {
            var resultObj = new QueryResult<BaseItemDto>
            {
                Items = new[]
                {
                    new SeasonBuilder("12", "23").BuildBaseItemDto()
                }
            };

            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = client.GetSeasons("123", DateTime.MinValue);
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0].Id.Should().Be(resultObj.Items[0].Id);

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(13);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();

            parameters.CheckQueryParameter(0, "AirDays", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(1, "EnableImageTypes", "Banner,Primary,Thumb,Logo", ParameterType.QueryString);
            parameters.CheckQueryParameter(2, "fields", "OriginalTitle,Genres,DateCreated,ExternalUrls,Studios,Path,Overview,ProviderIds,SortName,ParentId,People,MediaSources,MediaStreams,PremiereDate,CommunityRating,OfficialRating,ProductionYear,Status,RunTimeTicks", ParameterType.QueryString);
            parameters.CheckQueryParameter(3, "Filters", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(4, "ImageTypes", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(5, "IncludeItemTypes", "Season", ParameterType.QueryString);
            parameters.CheckQueryParameter(6, "LocationTypes", "FileSystem", ParameterType.QueryString);
            parameters.CheckQueryParameter(7, "ParentId", "123", ParameterType.QueryString);
            parameters.CheckQueryParameter(8, "recursive", "True", ParameterType.QueryString);
            parameters.CheckQueryParameter(9, "SeriesStatuses", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(10, "UserId", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246", ParameterType.QueryString);
            parameters.CheckQueryParameter(11, "X-Emby-Authorization", "mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"", ParameterType.HttpHeader);
            parameters.CheckQueryParameter(12, "X-Emby-Token", "apikey", ParameterType.HttpHeader);

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be("Items");
        }

        [Fact]
        public void GetEpisodes_Should_Return_List_Of_Episodes()
        {
            var resultObj = new QueryResult<BaseItemDto>
            {
                Items = new[]
                {
                    new EpisodeBuilder("12", "23", "1").BuildBaseItemDto()
                }
            };

            var client = CreateClient(resultObj);
            client.SetDeviceInfo("embystat", "mediabrowser", "0.0.0.0", "cb290477-d048-4b01-b201-8181922c6399", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            client.BaseUrl = "localhost:9000";
            client.ApiKey = "apikey";

            var result = client.GetEpisodes(new []{ "123" }, "12", DateTime.MinValue);
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            result[0].Id.Should().NotBeNullOrEmpty();

            _usedRequest.Should().NotBeNull();

            _usedRequest?.Parameters.Count.Should().Be(13);
            // ReSharper disable once PossibleNullReferenceException
            var parameters = _usedRequest.Parameters.OrderBy(x => x.Name).ToArray();

            parameters.CheckQueryParameter(0, "AirDays", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(1, "EnableImageTypes", "Banner,Primary,Thumb,Logo", ParameterType.QueryString);
            parameters.CheckQueryParameter(2, "fields", "OriginalTitle,Genres,DateCreated,ExternalUrls,Studios,Path,Overview,ProviderIds,SortName,ParentId,People,MediaSources,MediaStreams,PremiereDate,CommunityRating,OfficialRating,ProductionYear,Status,RunTimeTicks", ParameterType.QueryString);
            parameters.CheckQueryParameter(3, "Filters", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(4, "ImageTypes", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(5, "IncludeItemTypes", "Episode", ParameterType.QueryString);
            parameters.CheckQueryParameter(6, "LocationTypes", "FileSystem", ParameterType.QueryString);
            parameters.CheckQueryParameter(7, "ParentId", "123", ParameterType.QueryString);
            parameters.CheckQueryParameter(8, "recursive", "True", ParameterType.QueryString);
            parameters.CheckQueryParameter(9, "SeriesStatuses", string.Empty, ParameterType.QueryString);
            parameters.CheckQueryParameter(10, "UserId", "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246", ParameterType.QueryString);
            parameters.CheckQueryParameter(11, "X-Emby-Authorization", "mediabrowser RestClient=\"other\", DeviceId=\"cb290477-d048-4b01-b201-8181922c6399\", Device=\"embystat\", Version=\"0.0.0.0\"", ParameterType.HttpHeader);
            parameters.CheckQueryParameter(12, "X-Emby-Token", "apikey", ParameterType.HttpHeader);

            _usedRequest?.Method.Should().Be(Method.GET);
            _usedRequest?.Resource.Should().Be("Items");
        }
    }
}
