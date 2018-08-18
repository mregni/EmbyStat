using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers.ViewModels.Configuration;
using EmbyStat.Controllers.ViewModels.Task;
using EmbyStat.Repositories;
using EmbyStat.Web;
using FluentAssertions;
using MediaBrowser.Model.Plugins;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tests.Integration.Builders;
using Xunit;

namespace Tests.Integration
{
    public class ConfigurationControllerTest : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private ApplicationDbContext _context;

        public ConfigurationControllerTest()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());

            _client = _server.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:4123/api/configuration/");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void FillDatabase(bool withData)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlite("Data Source=data.db");

            _context = new ApplicationDbContext(builder.Options);
            _context.Configuration.RemoveRange(_context.Configuration.ToList());
            _context.SaveChanges();

            if (withData)
            {
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.EmbyUserId, Value = "433fbe4363a046ff9a11979ccd75aea8" });
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.LastTvdbUpdate, Value = "12/01/2018" });
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.AccessToken, Value = "1234567980" });
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.EmbyServerAddress, Value = "http://localhost" });
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.EmbyUserName, Value = "admin" });
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.Language, Value = "en-US" });
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.ServerName, Value = "Emby NAS" });
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.UserName, Value = "admin" });
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.WizardFinished, Value = "true" });
                _context.Configuration.Add(new ConfigurationKeyValue { Id = Constants.Configuration.ToShortMovie, Value = "10" });

                _context.SaveChanges();
            }
            else
            {
                var initlializer = new DatabaseInitializer(_context);
                Task.WaitAll(initlializer.SeedAsync());
            }
        }

        [Fact]
        public async void ShouldReturnFirstSeedConfiguration()
        {
            FillDatabase(false);
            var response = await _client.GetAsync("");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ConfigurationViewModel>(responseString);

            result.Should().NotBeNull();
            result.WizardFinished.Should().BeFalse();
            result.Language.Should().Be("en-US");
            result.ToShortMovie.Should().Be(10);
            result.AccessToken.Should().BeEmpty();
            result.EmbyServerAddress.Should().BeEmpty();
            result.EmbyUserId.Should().BeEmpty();
            result.EmbyUserName.Should().BeEmpty();
            result.ServerName.Should().BeEmpty();
            result.Username.Should().BeEmpty();
        }

        [Fact]
        public async void ShouldReturnFullConfiguration()
        {
            FillDatabase(true);
            var response = await _client.GetAsync("");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ConfigurationViewModel>(responseString);

            result.Should().NotBeNull();
            result.WizardFinished.Should().BeFalse();
            result.Language.Should().Be("en-US");
            result.ToShortMovie.Should().Be(10);
            result.AccessToken.Should().Be("1234567980");
            result.EmbyServerAddress.Should().Be("http://localhost");
            result.EmbyUserId.Should().Be("433fbe4363a046ff9a11979ccd75aea8");
            result.EmbyUserName.Should().Be("admin");
            result.ServerName.Should().Be("Emby NAS");
            result.Username.Should().Be("admin");
        }

        [Fact]
        public async void ShouldUpdateFullConfiguration()
        {
            FillDatabase(false);

            var configuration = new ConfigurationBuilder()
                .AddAccessToken("accessToken")
                .AddEmbyServerAddress("localhost")
                .AddEmbyUserId("69eb56e9fe6c43b5aec3723a43568860")
                .AddEmbyUserName("admin")
                .AddLanguage("nl-NL")
                .AddToShortMovie(15)
                .AddUserName("admin")
                .AddWizardFinished(true)
                .Build();

            var jsonTask = JsonConvert.SerializeObject(configuration);
            var content = new StringContent(jsonTask, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ConfigurationViewModel>(responseString);

            result.Should().NotBeNull();
            result.WizardFinished.Should().BeTrue();
            result.Language.Should().Be("nl-NL");
            result.ToShortMovie.Should().Be(15);
            result.AccessToken.Should().Be("accessToken");
            result.EmbyServerAddress.Should().Be("localhost");
            result.EmbyUserId.Should().Be("69eb56e9fe6c43b5aec3723a43568860");
            result.EmbyUserName.Should().Be("admin");
            result.Username.Should().Be("admin");
        }

        public void Dispose()
        {
            _context.Configuration.RemoveRange(_context.Configuration.ToList());
            _context.SaveChanges();
            _server?.Dispose();
            _client?.Dispose();
            Mapper.Reset();
        }
    }
}
