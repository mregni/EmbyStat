using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Controllers.ViewModels.Emby;
using EmbyStat.Repositories;
using EmbyStat.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace Tests.Integration
{
    public class PluginControllerTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public PluginControllerTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            builder.UseSqlite("Data Source=data.db");

            var context = new ApplicationDbContext(builder.Options);
            context.Database.Migrate();
        }

        [Fact]
        public async Task GetPlugins()
        {
            var response = await _client.GetAsync("/api/plugin/getdummy");
            string text = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<List<EmbyPluginViewModel>>(responseString);

            responseObj.Should().NotBeNull();
        }
    }
}
