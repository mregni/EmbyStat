using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Controllers.ViewModels.Emby;
using EmbyStat.Repositories;
using EmbyStat.Web;
using FluentAssertions;
using MediaBrowser.Model.Plugins;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Tests.Integration.Builders;
using Xunit;

namespace Tests.Integration
{
    public class PluginControllerTests : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private List<PluginInfo> _plugins;
        private ApplicationDbContext _context;
        public PluginControllerTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());

            _client = _server.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:4123/api/plugin");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            FillDatabase();
        }

        private void FillDatabase()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlite("Data Source=data.db");

            _context = new ApplicationDbContext(builder.Options);
            var initlializer = new DatabaseInitializer(_context);
            Task.WaitAll(initlializer.SeedAsync());

            _context.Plugins.RemoveRange(_context.Plugins.ToList());
            _context.SaveChanges();

            var pluginOne = new PluginBuilder()
                .AddAssemblyFileName("statistics.dll")
                .AddConfigurationDateLastModified(new DateTime(2018, 1, 1))
                .AddConfigurationFileName("statistics.config.xml")
                .AddDescription("Plugin for calculating statistics")
                .AddId("d0755cc879dc4aa586a3eefa97907b95")
                .AddImageUrl("http://google.com")
                .AddName("Statistics")
                .AddVersion("1.0.0.0")
                .Build();

            var pluginTwo = new PluginBuilder()
                .AddAssemblyFileName("cleaner.dll")
                .AddConfigurationDateLastModified(new DateTime(2018, 1, 1))
                .AddConfigurationFileName("cleaner.config.xml")
                .AddDescription("Plugin for cleaning up studios")
                .AddId("942ea4d13c7041a9bb098d988a267cc6")
                .AddImageUrl("http://google.com")
                .AddName("Studio Cleaner")
                .AddVersion("2.0.0.0")
                .Build();

            _plugins = new List<PluginInfo> { pluginOne, pluginTwo };
            _context.Plugins.AddRange(_plugins);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetPlugins()
        {
            var response = await _client.GetAsync("");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<EmbyPluginViewModel>>(responseString);

            result.Should().NotBeNull();
            result.Count.Should().Be(2);

            result.ElementAt(0).AssemblyFileName.Should().Be(_plugins[0].AssemblyFileName);
            result.ElementAt(0).ConfigurationDateLastModified.Should().Be(_plugins[0].ConfigurationDateLastModified);
            result.ElementAt(0).ConfigurationFileName.Should().Be(_plugins[0].ConfigurationFileName);
            result.ElementAt(0).Description.Should().Be(_plugins[0].Description);
            result.ElementAt(0).Id.Should().Be(_plugins[0].Id);
            result.ElementAt(0).ImageUrl.Should().Be(_plugins[0].ImageUrl);
            result.ElementAt(0).Name.Should().Be(_plugins[0].Name);
            result.ElementAt(0).Version.Should().Be(_plugins[0].Version);

            result.ElementAt(1).AssemblyFileName.Should().Be(_plugins[1].AssemblyFileName);
            result.ElementAt(1).ConfigurationDateLastModified.Should().Be(_plugins[1].ConfigurationDateLastModified);
            result.ElementAt(1).ConfigurationFileName.Should().Be(_plugins[1].ConfigurationFileName);
            result.ElementAt(1).Description.Should().Be(_plugins[1].Description);
            result.ElementAt(1).Id.Should().Be(_plugins[1].Id);
            result.ElementAt(1).ImageUrl.Should().Be(_plugins[1].ImageUrl);
            result.ElementAt(1).Name.Should().Be(_plugins[1].Name);
            result.ElementAt(1).Version.Should().Be(_plugins[1].Version);
        }

        public void Dispose()
        {
            _context.Plugins.RemoveRange(_context.Plugins.ToList());
            _context.SaveChanges();
            _server?.Dispose();
            _client?.Dispose();
        }
    }
}
