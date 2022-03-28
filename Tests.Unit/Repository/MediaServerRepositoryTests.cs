using System;
using EmbyStat.Common.Models.Entities;
using Xunit;

namespace Tests.Unit.Repository
{
    public class MediaServerRepositoryTests : BaseRepositoryTester
    {
        private MediaServerRepository _mediaServerRepository;
        private DbContext _context;
        public MediaServerRepositoryTests() : base("test-data-emby-repo.db")
        {
        }

        protected override void SetupRepository()
        {
            _context = CreateDbContext();
            _mediaServerRepository = new MediaServerRepository(_context);
        }

        [Fact]
        public void GetEmbyStatus_Should_Return_Current_Emby_Status()
        {
            RunTest(() =>
            {
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<MediaServerStatus>();
                    collection.Insert(new MediaServerStatus { Id = Guid.NewGuid(), MissedPings = 0 });
                }

                var status = _mediaServerRepository.GetEmbyStatus();
                status.Should().NotBeNull();
                status.MissedPings.Should().Be(0);
            });
        }

        [Fact]
        public void IncreaseMissedPings_Should_Increase_Missed_Pings_By_One()
        {
            RunTest(() =>
            {
                var status = new MediaServerStatus { Id = Guid.NewGuid(), MissedPings = 0 };
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<MediaServerStatus>();
                    collection.Insert(status);
                }

                _mediaServerRepository.IncreaseMissedPings();

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<MediaServerStatus>();
                    var dbStatus = collection.FindById(status.Id);
                    dbStatus.Should().NotBeNull();
                    dbStatus.Id.Should().Be(status.Id);
                    dbStatus.MissedPings.Should().Be(status.MissedPings + 1);
                }

            });
        }

        [Fact]
        public void ResetMissedPings_Should_Set_Missed_Pings_To_Zero()
        {
            RunTest(() =>
            {
                var status = new MediaServerStatus { Id = Guid.NewGuid(), MissedPings = 10 };
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<MediaServerStatus>();
                    collection.Insert(status);
                }

                _mediaServerRepository.ResetMissedPings();

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<MediaServerStatus>();
                    var dbStatus = collection.FindById(status.Id);
                    dbStatus.Should().NotBeNull();
                    dbStatus.Id.Should().Be(status.Id);
                    dbStatus.MissedPings.Should().Be(0);
                }
            });
        }

        [Fact]
        public void RemoveAllMediaServerData_Should_Remove_All_Data()
        {
            RunTest(() =>
            {
                using (var database = _context.LiteDatabase)
                {
                    var pluginOne = new PluginInfo { Id = Guid.NewGuid().ToString(), Name = "statistics" };
                    var pluginCollection = database.GetCollection<PluginInfo>();
                    pluginCollection.Insert(pluginOne);

                    var serverInfo = new ServerInfo { Id = Guid.NewGuid().ToString() };
                    var serverInfoCollection = database.GetCollection<ServerInfo>();
                    serverInfoCollection.Insert(serverInfo);

                    var embyUserOne = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "reggi" };
                    var userCollection = database.GetCollection<EmbyUser>();
                    userCollection.Insert(embyUserOne);

                    var serverOne = new Device { Id = Guid.NewGuid().ToString(), Name = "server1" };
                    var deviceCollection = database.GetCollection<Device>();
                    deviceCollection.Insert(serverOne);
                }

                _mediaServerRepository.RemoveAllMediaServerData();

                using (var database = _context.LiteDatabase)
                {
                    var pluginCollection = database.GetCollection<PluginInfo>();
                    var plugins = pluginCollection.FindAll();
                    plugins.Count().Should().Be(0);

                    var serverInfoCollection = database.GetCollection<ServerInfo>();
                    var serverInfo = serverInfoCollection.FindAll();
                    serverInfo.Count().Should().Be(0);

                    var userCollection = database.GetCollection<EmbyUser>();
                    var users = userCollection.FindAll();
                    users.Count().Should().Be(0);

                    var deviceCollection = database.GetCollection<Device>();
                    var device = deviceCollection.FindAll();
                    device.Count().Should().Be(0);
                }
            });
        }

        [Fact]
        public void GetAllPlugins_Should_Return_All_Plugins()
        {
            RunTest(() =>
            {
                var pluginOne = new PluginInfo { Id = Guid.NewGuid().ToString(), Name = "statistics" };
                var pluginTwo = new PluginInfo { Id = Guid.NewGuid().ToString(), Name = "movies" };
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<PluginInfo>();
                    collection.InsertBulk(new[] { pluginOne, pluginTwo });
                }

                var plugins = _mediaServerRepository.GetAllPlugins();
                plugins.Should().NotContainNulls();
                plugins.Count.Should().Be(2);

                plugins[0].Id.Should().Be(pluginTwo.Id);
                plugins[0].Name.Should().Be(pluginTwo.Name);

                plugins[1].Id.Should().Be(pluginOne.Id);
                plugins[1].Name.Should().Be(pluginOne.Name);
            });
        }

        [Fact]
        public void RemoveAllAndInsertPluginRange_Should_Remove_Old_And_Insert_New_Plugins()
        {
            RunTest(() =>
            {
                var pluginOne = new PluginInfo { Id = Guid.NewGuid().ToString(), Name = "statistics" };
                var pluginTwo = new PluginInfo { Id = Guid.NewGuid().ToString(), Name = "movies" };
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<PluginInfo>();
                    collection.InsertBulk(new[] { pluginOne, pluginTwo });
                }

                var pluginThree = new PluginInfo { Id = Guid.NewGuid().ToString(), Name = "shows" };
                var pluginFour = new PluginInfo { Id = Guid.NewGuid().ToString(), Name = "tvdb" };

                _mediaServerRepository.RemoveAllAndInsertPluginRange(new[] { pluginThree, pluginFour });

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<PluginInfo>();
                    var plugins = collection.FindAll().OrderBy(x => x.Name).ToList();

                    plugins.Should().NotContainNulls();
                    plugins.Count.Should().Be(2);

                    plugins[0].Id.Should().Be(pluginThree.Id);
                    plugins[0].Name.Should().Be(pluginThree.Name);

                    plugins[1].Id.Should().Be(pluginFour.Id);
                    plugins[1].Name.Should().Be(pluginFour.Name);
                }
            });
        }

        [Fact]
        public void GetServerInfo_Should_Return_Server_Info()
        {
            RunTest(() =>
            {
                var serverInfo = new ServerInfo { Id = Guid.NewGuid().ToString() };
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<ServerInfo>();
                    collection.Insert(serverInfo);
                }

                var serverInfoDb = _mediaServerRepository.GetServerInfo();

                serverInfoDb.Should().NotBeNull();
                serverInfoDb.Id.Should().Be(serverInfo.Id);

            });
        }

        [Fact]
        public void UpsertServerInfo_Should_Insert_The_Server_Info()
        {
            RunTest(() =>
            {
                var serverInfo = new ServerInfo { Id = Guid.NewGuid().ToString() };
                _mediaServerRepository.UpsertServerInfo(serverInfo);

                using var database = _context.LiteDatabase;
                var collection = database.GetCollection<ServerInfo>();
                var serverInfoDb = collection.FindById(serverInfo.Id);

                serverInfoDb.Should().NotBeNull();
                serverInfoDb.Id.Should().Be(serverInfo.Id);
            });
        }

        [Fact]
        public void UpsertServerInfo_Should_Update_The_Server_Info()
        {
            RunTest(() =>
            {
                var serverInfo = new ServerInfo { Id = Guid.NewGuid().ToString() };
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<ServerInfo>();
                    collection.Insert(serverInfo);

                    var serverInfoDb = collection.FindById(serverInfo.Id);

                    serverInfoDb.Should().NotBeNull();
                    serverInfoDb.Id.Should().Be(serverInfo.Id);
                }

                serverInfo.CachePath = "/temp";
                _mediaServerRepository.UpsertServerInfo(serverInfo);

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<ServerInfo>();
                    var serverInfoDb = collection.FindById(serverInfo.Id);

                    serverInfoDb.Should().NotBeNull();
                    serverInfoDb.Id.Should().Be(serverInfo.Id);
                    serverInfoDb.CachePath.Should().Be(serverInfo.CachePath);
                }
            });
        }

        [Fact]
        public void GetAllUsers_Should_Return_All_Users()
        {
            RunTest(() =>
            {
                var embyUserOne = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "reggi", IsAdministrator = false};
                var embyUserTwo = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "tom", IsAdministrator = true};
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<EmbyUser>();
                    collection.InsertBulk(new[] { embyUserOne, embyUserTwo });
                }

                var users = _mediaServerRepository.GetAllUsers().ToList();
                users.Should().NotContainNulls();
                users.Count.Should().Be(2);

                users[0].Id.Should().Be(embyUserOne.Id);
                users[0].Name.Should().Be(embyUserOne.Name);

                users[1].Id.Should().Be(embyUserTwo.Id);
                users[1].Name.Should().Be(embyUserTwo.Name);
            });
        }

        [Fact]
        public void GetAllAdministrators_Should_Return_All_Administrators()
        {
            RunTest(() =>
            {
                var embyUserOne = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "reggi", IsAdministrator = false};
                var embyUserTwo = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "tom", IsAdministrator = true};
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<EmbyUser>();
                    collection.InsertBulk(new[] { embyUserOne, embyUserTwo });
                }

                var users = _mediaServerRepository.GetAllAdministrators().ToList();
                users.Should().NotContainNulls();
                users.Count.Should().Be(1);

                users[0].Id.Should().Be(embyUserTwo.Id);
                users[0].Name.Should().Be(embyUserTwo.Name);
            });
        }

        [Fact]
        public void UpsertUsers_Should_Insert_New_Users()
        {
            RunTest(() =>
            {
                var embyUserOne = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "reggi" };
                var embyUserTwo = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "tom" };
                _mediaServerRepository.UpsertUsers(new[] { embyUserOne, embyUserTwo });

                using var database = _context.LiteDatabase;
                var collection = database.GetCollection<EmbyUser>();
                var users = collection.FindAll().OrderBy(x => x.Name).ToList();

                users.Should().NotContainNulls();
                users.Count.Should().Be(2);

                users[0].Id.Should().Be(embyUserOne.Id);
                users[0].Name.Should().Be(embyUserOne.Name);

                users[1].Id.Should().Be(embyUserTwo.Id);
                users[1].Name.Should().Be(embyUserTwo.Name);
            });
        }

        [Fact]
        public void UpsertUsers_Should_Update_Existing_Users()
        {
            RunTest(() =>
            {
                var embyUserOne = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "reggi" };
                var embyUserTwo = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "tom" };

                _mediaServerRepository.UpsertUsers(new[] { embyUserTwo });

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<EmbyUser>();
                    var users = collection.FindAll().OrderBy(x => x.Name).ToList();

                    users.Should().NotContainNulls();
                    users.Count.Should().Be(1);

                    users[0].Id.Should().Be(embyUserTwo.Id);
                    users[0].Name.Should().Be(embyUserTwo.Name);
                }

                embyUserTwo.Name = "tim";

                _mediaServerRepository.UpsertUsers(new[] { embyUserOne, embyUserTwo });

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<EmbyUser>();
                    var users = collection.FindAll().OrderBy(x => x.Name).ToList();

                    users.Should().NotContainNulls();
                    users.Count.Should().Be(2);

                    users[0].Id.Should().Be(embyUserOne.Id);
                    users[0].Name.Should().Be(embyUserOne.Name);

                    users[1].Id.Should().Be(embyUserTwo.Id);
                    users[1].Name.Should().Be(embyUserTwo.Name);
                }
            });
        }

        [Fact]
        public void MarkUsersAsDeleted_Should_Mark_Users_As_Deleted()
        {
            RunTest(() =>
            {
                var embyUserOne = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "reggi" };
                var embyUserTwo = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "tom" };
                var embyUserThee = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "yol" };
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<EmbyUser>();
                    collection.InsertBulk(new[] { embyUserOne, embyUserTwo, embyUserThee });
                }

                _mediaServerRepository.MarkUsersAsDeleted(new[] { embyUserOne, embyUserTwo });

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<EmbyUser>();
                    var users = collection.FindAll().OrderBy(x => x.Name).ToList();

                    users.Should().NotContainNulls();
                    users.Count.Should().Be(3);

                    users[0].Id.Should().Be(embyUserOne.Id);
                    users[0].Name.Should().Be(embyUserOne.Name);
                    users[0].Deleted.Should().BeTrue();

                    users[1].Id.Should().Be(embyUserTwo.Id);
                    users[1].Name.Should().Be(embyUserTwo.Name);
                    users[1].Deleted.Should().BeTrue();

                    users[2].Id.Should().Be(embyUserThee.Id);
                    users[2].Name.Should().Be(embyUserThee.Name);
                    users[2].Deleted.Should().BeFalse();
                }
            });
        }

        [Fact]
        public void GetUserById_Should_Return_Correct_User()
        {
            RunTest(() =>
            {
                var embyUserOne = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "reggi" };
                var embyUserTwo = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "tom" };
                var embyUserThee = new EmbyUser { Id = Guid.NewGuid().ToString(), Name = "yol" };
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<EmbyUser>();
                    collection.InsertBulk(new[] { embyUserOne, embyUserTwo, embyUserThee });
                }

                var user = _mediaServerRepository.GetUserById(embyUserTwo.Id);

                user.Should().NotBeNull();
                user.Id.Should().Be(embyUserTwo.Id);
                user.Name.Should().Be(embyUserTwo.Name);
            });
        }

        #region Devices

        [Fact]
        public void GetAllDevices_Should_Return_All_Devices()
        {
            RunTest(() =>
            {
                var serverOne = new Device { Id = Guid.NewGuid().ToString(), Name = "server1" };
                var serverTwo = new Device { Id = Guid.NewGuid().ToString(), Name = "server2" };
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Device>();
                    collection.InsertBulk(new[] { serverOne, serverTwo });
                }

                var devices = _mediaServerRepository.GetAllDevices();
                devices.Should().NotContainNulls();
                devices.Count.Should().Be(2);

                devices[0].Id.Should().Be(serverOne.Id);
                devices[0].Name.Should().Be(serverOne.Name);

                devices[1].Id.Should().Be(serverTwo.Id);
                devices[1].Name.Should().Be(serverTwo.Name);
            });
        }

        [Fact]
        public void UpsertDevice_Should_Insert_New_Devices()
        {
            RunTest(() =>
            {
                var serverOne = new Device { Id = Guid.NewGuid().ToString(), Name = "server1" };
                var serverTwo = new Device { Id = Guid.NewGuid().ToString(), Name = "server2" };
                _mediaServerRepository.UpsertDevices(new[] { serverOne, serverTwo });

                using var database = _context.LiteDatabase;
                var collection = database.GetCollection<Device>();
                var devices = collection.FindAll().OrderBy(x => x.Name).ToList();

                devices.Should().NotContainNulls();
                devices.Count.Should().Be(2);

                devices[0].Id.Should().Be(serverOne.Id);
                devices[0].Name.Should().Be(serverOne.Name);

                devices[1].Id.Should().Be(serverTwo.Id);
                devices[1].Name.Should().Be(serverTwo.Name);
            });
        }

        [Fact]
        public void UpsertDevice_Should_Update_Existing_Device()
        {
            RunTest(() =>
            {
                var serverOne = new Device { Id = Guid.NewGuid().ToString(), Name = "server1" };
                var serverTwo = new Device { Id = Guid.NewGuid().ToString(), Name = "server2" };

                _mediaServerRepository.UpsertDevices(new[] { serverTwo });

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Device>();
                    var devices = collection.FindAll().OrderBy(x => x.Name).ToList();
                    
                    devices.Should().NotContainNulls();
                    devices.Count.Should().Be(1);

                    devices[0].Id.Should().Be(serverTwo.Id);
                    devices[0].Name.Should().Be(serverTwo.Name);
                }

                serverTwo.Name = "server3";

                _mediaServerRepository.UpsertDevices(new[] { serverOne, serverTwo });

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Device>();
                    var devices = collection.FindAll().OrderBy(x => x.Name).ToList();

                    devices.Should().NotContainNulls();
                    devices.Count.Should().Be(2);

                    devices[0].Id.Should().Be(serverOne.Id);
                    devices[0].Name.Should().Be(serverOne.Name);

                    devices[1].Id.Should().Be(serverTwo.Id);
                    devices[1].Name.Should().Be(serverTwo.Name);
                }
            });
        }

        [Fact]
        public void MarkDevicesAsDeleted_Should_Mark_Device_As_Deleted()
        {
            RunTest(() =>
            {
                var serverOne = new Device { Id = Guid.NewGuid().ToString(), Name = "server1" };
                var serverTwo = new Device { Id = Guid.NewGuid().ToString(), Name = "server2" };
                var serverThee = new Device { Id = Guid.NewGuid().ToString(), Name = "server3" };
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Device>();
                    collection.InsertBulk(new[] { serverOne, serverTwo, serverThee });
                }

                _mediaServerRepository.MarkDevicesAsDeleted(new[] { serverOne, serverTwo });

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Device>();
                    var devices = collection.FindAll().OrderBy(x => x.Name).ToList();

                    devices.Should().NotContainNulls();
                    devices.Count.Should().Be(3);

                    devices[0].Id.Should().Be(serverOne.Id);
                    devices[0].Name.Should().Be(serverOne.Name);
                    devices[0].Deleted.Should().BeTrue();

                    devices[1].Id.Should().Be(serverTwo.Id);
                    devices[1].Name.Should().Be(serverTwo.Name);
                    devices[1].Deleted.Should().BeTrue();

                    devices[2].Id.Should().Be(serverThee.Id);
                    devices[2].Name.Should().Be(serverThee.Name);
                    devices[2].Deleted.Should().BeFalse();
                }
            });
        }

        [Fact]
        public void GetDevicesById_Should_Return_Correct_Device()
        {
            RunTest(() =>
            {
                var serverOne = new Device { Id = Guid.NewGuid().ToString(), Name = "server1" };
                var serverTwo = new Device { Id = Guid.NewGuid().ToString(), Name = "server2" };
                var serverThee = new Device { Id = Guid.NewGuid().ToString(), Name = "server3" };
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Device>();
                    collection.InsertBulk(new[] { serverOne, serverTwo, serverThee });
                }

                var devices = _mediaServerRepository.GetDeviceById(new []{ serverTwo.Id });

                devices.Should().NotContainNulls();
                devices.Count.Should().Be(1);
                devices[0].Should().NotBeNull();
                devices[0].Id.Should().Be(serverTwo.Id);
                devices[0].Name.Should().Be(serverTwo.Name);
            });
        }


        #endregion
    }
}
