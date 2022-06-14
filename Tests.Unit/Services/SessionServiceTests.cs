//TODO: fix
// using System.Collections.Generic;
// using System.Linq;
// using EmbyStat.Common.Enums;
// using EmbyStat.Common.Models.Entities.Events;
// using EmbyStat.Services;
// using FluentAssertions;
// using Moq;
// using Xunit;
//
// namespace Tests.Unit.Services
// {
//     public class SessionServiceTests
//     {
//         private readonly SessionService _sessionService;
//         private readonly Mock<ISessionRepository> _sessionRepositoryObject;
//
//         private readonly List<string> _mediaIds;
//         private readonly List<Session> _sessions;
//
//         public SessionServiceTests()
//         {
//             _mediaIds = new List<string>
//             {
//                 "111", "112", "113"
//             };
//
//             _sessions = new List<Session>
//             {
//                 new() { Id = "1" }
//             };
//
//             _sessionRepositoryObject = new Mock<ISessionRepository>();
//             _sessionRepositoryObject
//                 .Setup(x => x.GetMediaIdsForUser(It.IsAny<string>(), It.IsAny<PlayType>()))
//                 .Returns(_mediaIds);
//             _sessionRepositoryObject
//                 .Setup(x => x.GetSessionsForUser(It.IsAny<string>()))
//                 .Returns(_sessions);
//
//             _sessionService = new SessionService(_sessionRepositoryObject.Object);
//         }
//
//         [Fact]
//         public void GetMediaIdsForUser_Should_Return_Media_Ids()
//         {
//             var ids = _sessionService.GetMediaIdsForUser("1", PlayType.Movie).ToList();
//
//             ids.Count.Should().Be(3);
//             ids[0].Should().Be(_mediaIds[0]);
//             ids[1].Should().Be(_mediaIds[1]);
//             ids[2].Should().Be(_mediaIds[2]);
//
//             _sessionRepositoryObject.Verify(x => x.GetMediaIdsForUser("1", PlayType.Movie), Times.Exactly(1));
//         }
//
//         [Fact]
//         public void GetSessionsForUser_Should_Return_Sessions()
//         {
//             var ids = _sessionService.GetSessionsForUser("1").ToList();
//
//             ids.Count.Should().Be(1);
//             ids[0].Should().Be(_sessions[0]);
//
//             _sessionRepositoryObject.Verify(x => x.GetSessionsForUser("1"), Times.Exactly(1));
//         }
//     }
// }
