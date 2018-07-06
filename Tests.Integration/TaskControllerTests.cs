using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Exceptions;
using EmbyStat.Controllers.ViewModels.Emby;
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
using Tests.Integration.Helpers;
using Xunit;

namespace Tests.Integration
{
    public class TaskControllerTests : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private ApplicationDbContext _context;

        public TaskControllerTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());

            _client = _server.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:4123/api/task/");
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
        }

        [Fact]
        public async void ShouldReturnAllTasks()
        {
            var response = await _client.GetAsync("");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<TaskInfoViewModel>>(responseString);

            result.Should().NotBeNull();
            result.Count.Should().Be(3);

            result.ElementAt(0).Category.Should().Be("Emby");
            result.ElementAt(0).CurrentProgressPercentage.Should().BeNull();
            result.ElementAt(0).Description.Should().Be("TASKS.PINGEMBYSERVERDESCRIPTION");
            result.ElementAt(0).LastExecutionResult.Should().BeNull();
            result.ElementAt(0).Name.Should().Be("Check Emby connection");
            result.ElementAt(0).State.Should().Be(0);
            result.ElementAt(0).Triggers.Count.Should().Be(1);
            result.ElementAt(0).Triggers.ElementAt(0).Id.Should().BeNull();
            result.ElementAt(0).Triggers.ElementAt(0).Type.Should().Be("IntervalTrigger");
            result.ElementAt(0).Triggers.ElementAt(0).TimeOfDayTicks.Should().BeNull();
            result.ElementAt(0).Triggers.ElementAt(0).IntervalTicks.Should().Be(1200000000);
            result.ElementAt(0).Triggers.ElementAt(0).DayOfWeek.Should().BeNull();
            result.ElementAt(0).Triggers.ElementAt(0).MaxRuntimeTicks.Should().BeNull();

            result.ElementAt(1).Category.Should().Be("Sync");
            result.ElementAt(1).CurrentProgressPercentage.Should().BeNull();
            result.ElementAt(1).Description.Should().Be("TASKS.MOVIESYNCDESCRIPTION");
            result.ElementAt(1).LastExecutionResult.Should().BeNull();
            result.ElementAt(1).Name.Should().Be("Movie sync");
            result.ElementAt(1).State.Should().Be(0);
            result.ElementAt(1).Triggers.Count.Should().Be(1);
            result.ElementAt(1).Triggers.ElementAt(0).Id.Should().BeNull();
            result.ElementAt(1).Triggers.ElementAt(0).Type.Should().Be("DailyTrigger");
            result.ElementAt(1).Triggers.ElementAt(0).TimeOfDayTicks.Should().Be(0);
            result.ElementAt(1).Triggers.ElementAt(0).IntervalTicks.Should().BeNull();
            result.ElementAt(1).Triggers.ElementAt(0).DayOfWeek.Should().BeNull();
            result.ElementAt(1).Triggers.ElementAt(0).MaxRuntimeTicks.Should().BeNull();

            result.ElementAt(2).Category.Should().Be("Emby");
            result.ElementAt(2).CurrentProgressPercentage.Should().BeNull();
            result.ElementAt(2).Description.Should().Be("TASKS.SMALLEMBYSYNCDESCRIPTION");
            result.ElementAt(2).LastExecutionResult.Should().BeNull();
            result.ElementAt(2).Name.Should().Be("Small sync with Emby");
            result.ElementAt(2).State.Should().Be(0);
            result.ElementAt(2).Triggers.Count.Should().Be(1);
            result.ElementAt(2).Triggers.ElementAt(0).Id.Should().BeNull();
            result.ElementAt(2).Triggers.ElementAt(0).Type.Should().Be("DailyTrigger");
            result.ElementAt(2).Triggers.ElementAt(0).TimeOfDayTicks.Should().Be(18000000000);
            result.ElementAt(2).Triggers.ElementAt(0).IntervalTicks.Should().BeNull();
            result.ElementAt(2).Triggers.ElementAt(0).DayOfWeek.Should().BeNull();
            result.ElementAt(2).Triggers.ElementAt(0).MaxRuntimeTicks.Should().BeNull();
        }

        [Fact]
        public async void ShouldAddNewTriggersToTask()
        {
            var response = await _client.GetAsync("");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var tasks = JsonConvert.DeserializeObject<List<TaskInfoViewModel>>(responseString);

            tasks.Count.Should().Be(3);
            tasks.First().Triggers.Count.Should().Be(1);
            var task = tasks.First();

            var newTriggerOne = new TaskTriggerInfoBuilder()
                .AddId("6ce2961ef6ef455e8bb34eb4254bca3d")
                .AddIntervalTicks(1000000)
                .AddType("IntervalTrigger")
                .Build();

            var newTriggerTwo = new TaskTriggerInfoBuilder()
                .AddId("82df2551a1694cdb8c93e529f6926df9")
                .AddTimeOfDayTicks(10)
                .AddType("DailyTrigger")
                .Build();

            task.Triggers.Add(newTriggerOne);
            task.Triggers.Add(newTriggerTwo);

            var jsonTask = JsonConvert.SerializeObject(task);
            var content = new StringContent(jsonTask, Encoding.UTF8, "application/json");

            var updatedResponse = await _client.PutAsync("triggers", content);
            updatedResponse.EnsureSuccessStatusCode();

            var updatedResponseString = await updatedResponse.Content.ReadAsStringAsync();
            tasks = JsonConvert.DeserializeObject<List<TaskInfoViewModel>>(updatedResponseString);

            tasks.Should().NotBeNull();
            task = tasks.First(x => x.Id == task.Id);
            task.Triggers.Count.Should().Be(3);
            task.Triggers.Any(x => x.Id == "6ce2961ef6ef455e8bb34eb4254bca3d").Should().BeTrue();
            task.Triggers.Any(x => x.Id == "82df2551a1694cdb8c93e529f6926df9").Should().BeTrue();
        }

        [Fact]
        public async void ShouldCrashBecauseWrongTaskId()
        {
            var jsonTask = JsonConvert.SerializeObject(new TaskInfoViewModel(){ Id = Guid.NewGuid().ToString()});
            var content = new StringContent(jsonTask, Encoding.UTF8, "application/json");

            var result = await _client.PutAsync("triggers", content);
            var errorString = await result.Content.ReadAsStringAsync();
            var error = JsonConvert.DeserializeObject<ExceptionMessage>(errorString);

            result.StatusCode.Should().Be(500);
            error.Message.Should().Be("INTERNAL_ERROR");
        }

        public void Dispose()
        {
            _context.Plugins.RemoveRange(_context.Plugins.ToList());
            _context.SaveChanges();
            _server?.Dispose();
            _client?.Dispose();
            Mapper.Reset();
        }
    }
}
