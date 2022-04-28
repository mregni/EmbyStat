using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Services;

public class LanguageServiceTests
{
    private readonly List<Language> _languages;
    private readonly LanguageService _languageService;
    private readonly Mock<ILanguageRepository> _languageRepositoryMock;

    public LanguageServiceTests()
    {
        _languages = new List<Language>
        {
            new() {Name = "Nederlands", Code = "nl-BE", Id = "100"},
            new() {Name = "English", Code = "en-US", Id = "101"}
        };

        _languageRepositoryMock = new Mock<ILanguageRepository>();
        _languageRepositoryMock.Setup(x => x.GetLanguages()).ReturnsAsync(_languages);

        _languageService = new LanguageService(_languageRepositoryMock.Object);
    }

    [Fact]
    public async Task GetLanguages()
    {
        var languages = await _languageService.GetLanguages();

        languages.Should().NotBeNull();
        languages.Count.Should().Be(_languages.Count);

        languages[0].Name.Should().Be("Nederlands");
        languages[0].Code.Should().Be("nl-BE");
        languages[0].Id.Should().Be("100");

        languages[1].Name.Should().Be("English");
        languages[1].Code.Should().Be("en-US");
        languages[1].Id.Should().Be("101");

        _languageRepositoryMock.Verify(x => x.GetLanguages(), Times.Exactly(1));
        _languageRepositoryMock.VerifyNoOtherCalls();
    }
}