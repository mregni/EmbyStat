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
using Xunit;

namespace Tests.Integration
{
    public class PluginControllerTests : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly List<PluginInfo> _plugins;
        private readonly ApplicationDbContext _context;
        public PluginControllerTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseContentRoot("..\\..\\..\\..\\EmbyStat.Web")
                .UseStartup<Startup>());

            _client = _server.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:4123");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            builder.UseSqlite("Data Source=data.db");

            _context = new ApplicationDbContext(builder.Options);
            var initlializer = new DatabaseInitializer(_context);
            Task.WaitAll(initlializer.SeedAsync());

            _plugins = new List<PluginInfo>
            {
                new PluginInfo{ Name = "Statistics", Version = "1.0.0.0", Id = "e74908335a5042bf9c9391dbebd548fd"},
                new PluginInfo{ Name = "Studio Cleaner", Version = "2.0.0.0", Id = "942ea4d13c7041a9bb098d988a267cc6"}
            };
            //_context.Plugins.AddRange(_plugins);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetPlugins()
        {
            var response = await _client.GetAsync("/dummy");
            var text = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<List<EmbyPluginViewModel>>(responseString);

            responseObj.Should().NotBeNull();
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
