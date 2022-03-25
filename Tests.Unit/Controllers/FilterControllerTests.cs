using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Controllers;
using EmbyStat.Controllers.Filters;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
    public class FilterControllerTests
    {
        private readonly FilterController _controller;

        public FilterControllerTests()
        {
            var profiles = new MapProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profiles));
            var mapper = new Mapper(configuration);

            var values = new FilterValues
            {
                Id = "1",
                Field = "2",
                Values = new[] { new LabelValuePair { Label = "3", Value = "3" }, new LabelValuePair { Label = "4", Value = "4" } },
                Libraries = new [] { "5", "6" }
            };

            var filterServiceMock = new Mock<IFilterService>();
            filterServiceMock
                .Setup(x => x.GetFilterValues(It.IsAny<LibraryType>(), It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(values);

            _controller = new FilterController(filterServiceMock.Object, mapper);
        }

        [Fact]
        public void Get_Should_Return_A_Proper_FilterValues_Object()
        {
            var result = _controller.Get(LibraryType.Movies, "subtitle", new[] {"1"});
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var viewModel = resultObject.Should().BeOfType<FilterValuesViewModel>().Subject;

            viewModel.Id.Should().Be("1");
            viewModel.Field.Should().Be("2");
            viewModel.Values.Length.Should().Be(2);
            viewModel.Values[0].Label.Should().Be("3");
            viewModel.Values[0].Value.Should().Be("3");
            viewModel.Values[1].Label.Should().Be("4");
            viewModel.Values[1].Value.Should().Be("4");
        }
    }
}
