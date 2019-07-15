using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Enums;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
	[Collection("Mapper collection")]
    public class SessionServiceTests
    {
        private readonly SessionService _sessionService;
        private readonly Mock<ISessionRepository> _sessionRepositoryObject;

        private readonly List<string> _mediaIds;

        public SessionServiceTests()
        {
            _mediaIds = new List<string>
            {
                "111", "112", "113"
            };

            _sessionRepositoryObject = new Mock<ISessionRepository>();
            _sessionRepositoryObject
                .Setup(x => x.GetMediaIdsForUser(It.IsAny<string>(), It.IsAny<PlayType>()))
                .Returns(_mediaIds);

            _sessionService = new SessionService(_sessionRepositoryObject.Object);
        }

        [Fact]
        public void GetMediaIdsForAUser()
        {
            var ids = _sessionService.GetMediaIdsForUser("1", PlayType.Movie).ToList();

            ids.Count.Should().Be(3);
            ids[0].Should().Be(_mediaIds[0]);
            ids[1].Should().Be(_mediaIds[1]);
            ids[2].Should().Be(_mediaIds[2]);

            _sessionRepositoryObject.Verify(x => x.GetMediaIdsForUser("1", PlayType.Movie), Times.Exactly(1));
        }
    }
}
