using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.EmbyClient;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using Newtonsoft.Json;
using Serilog;

namespace EmbyStat.Services
{
    public class EmbyService : IEmbyService
    {
	    private readonly IEmbyClient _embyClient;
        private readonly IEmbyRepository _embyRepository;
        private readonly ISettingsService _settingsService;
        private readonly IMapper _mapper;

        public EmbyService(IEmbyClient embyClient, ISettingsService settingsService, IEmbyRepository embyRepository, IMapper mapper)
        {
            _embyClient = embyClient;
            _settingsService = settingsService;
            _embyRepository = embyRepository;
            _mapper = mapper;
        }

        #region Server
        public EmbyUdpBroadcast SearchEmby()
	    {
		    using (var client = new UdpClient())
		    {
			    var requestData = Encoding.ASCII.GetBytes("who is EmbyServer?");
			    var serverEp = new IPEndPoint(IPAddress.Any, 7359);

			    client.EnableBroadcast = true;
			    client.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, 7359));

			    var timeToWait = TimeSpan.FromSeconds(2);

			    var asyncResult = client.BeginReceive(null, null);
			    asyncResult.AsyncWaitHandle.WaitOne(timeToWait);
			    if (asyncResult.IsCompleted)
			    {
				    try
				    {
					    var receivedData = client.EndReceive(asyncResult, ref serverEp);
					    var serverResponse = Encoding.ASCII.GetString(receivedData);
						var udpBroadcastResult = JsonConvert.DeserializeObject<EmbyUdpBroadcast>(serverResponse);

                        var settings = _settingsService.GetUserSettings();
					    settings.Emby.ServerName = udpBroadcastResult.Name;
						_settingsService.SaveUserSettings(settings);

					    return udpBroadcastResult;

				    }
				    catch (Exception)
				    {
					    // No data recieved, swallow exception and return empty object
				    }
			    }

			    return new EmbyUdpBroadcast();
		    }
	    }

	    public async Task<EmbyToken> GetEmbyToken(EmbyLogin login)
		{
		    if (!string.IsNullOrEmpty(login?.Password) && !string.IsNullOrEmpty(login.UserName))
		    {
				try
				{
                    //TODO, fix this code
					var token = await _embyClient.AuthenticateUserAsync(login.UserName, login.Password, login.Address);
					return new EmbyToken
					{
						Token = token.AccessToken,
						Username = token.User.ConnectUserName,
						IsAdmin = token.User.Policy.IsAdministrator,
                        Id = new Guid(token.User.Id)
					};
				}
				catch (Exception e)
				{
					Log.Warning($"{Constants.LogPrefix.ServerApi}\tUsername or password are wrong, user should try again with other credentials!");
					throw new BusinessException("TOKEN_FAILED");
				}
			}

		    Log.Error("Username or password are empty, no use to try a login!");
			throw new BusinessException("TOKEN_FAILED");
	    }

	    public async Task<ServerInfo> GetServerInfo()
	    {
		    var server = _embyRepository.GetServerInfo();
            if (server == null)
            {
                var settings = _settingsService.GetUserSettings();
                await GetAndProcessServerInfo(settings.FullEmbyServerAddress, settings.Emby.AccessToken);
            }

            return server;
        }

        public List<Drive> GetLocalDrives()
        {
            return _embyRepository.GetAllDrives();
        }

        public EmbyStatus GetEmbyStatus()
        {
            return _embyRepository.GetEmbyStatus();
        }

        public async Task<string> PingEmbyAsync(string embyAddress, string accessToken, CancellationToken cancellationToken)
        {
            _embyClient.SetAddressAndUrl(embyAddress, accessToken);
            return await _embyClient.PingEmbyAsync(cancellationToken);
        }

        public void ResetMissedPings()
        {
             _embyRepository.ResetMissedPings();
        }

        public void IncreaseMissedPings()
        {
            _embyRepository.IncreaseMissedPings();
        }

        #endregion

        #region Plugin

        public List<PluginInfo> GetAllPlugins()
        {
            return _embyRepository.GetAllPlugins();
        }

        #endregion

        #region Users

        public IEnumerable<User> GetAllUsers()
        {
            return _embyRepository.GetAllUsers();
        }

        public User GetUserById(string id)
        {
            return _embyRepository.GetUserById(id);
        }

        #endregion

        #region JobHelpers

        public async Task GetAndProcessServerInfo(string embyAddress, string accessToken)
        {
            _embyClient.SetAddressAndUrl(embyAddress, accessToken);
            var server = await _embyClient.GetServerInfoAsync();

            _embyRepository.AddOrUpdateServerInfo(server);
        }

        public async Task GetAndProcessPluginInfo(string embyAddress, string accessToken)
        {
            _embyClient.SetAddressAndUrl(embyAddress, accessToken);
            var plugins = await _embyClient.GetInstalledPluginsAsync();

            _embyRepository.RemoveAllAndInsertPluginRange(_mapper.Map<IList<PluginInfo>>(plugins));
        }

        public async Task GetAndProcessEmbyDriveInfo(string embyAddress, string accessToken)
        {
            _embyClient.SetAddressAndUrl(embyAddress, accessToken);
            var drives = await _embyClient.GetLocalDrivesAsync();

            _embyRepository.RemoveAllAndInsertDriveRange(_mapper.Map<IList<Drive>>(drives));
        }

        public async Task GetAndProcessEmbyUsers(string embyAddress, string accessToken)
        {
            _embyClient.SetAddressAndUrl(embyAddress, accessToken);

            var usersJson = await _embyClient.GetEmbyUsers();
            var users = UserConverter.ConvertToUserList(usersJson).ToList();
            await _embyRepository.AddOrUpdateUsers(users);

            var localUsers = _embyRepository.GetAllUsers();
            var removedUsers = localUsers.Where(u => users.All(u2 => u2.Id != u.Id));
            await _embyRepository.MarkUserAsDeleted(removedUsers);
        }

        public async Task GetAndProcessDevices(string embyAddress, string accessToken)
        {
            _embyClient.SetAddressAndUrl(embyAddress, accessToken);
            var devicesJson = await _embyClient.GetEmbyDevices();
            var devices = DeviceConverter.ConvertToDeviceList(devicesJson).ToList();
            await _embyRepository.AddOrUpdateDevices(devices);

            var localDevices = _embyRepository.GetAllDevices();
            var removedDevices = localDevices.Where(d => devices.All(d2 => d2.Id != d.Id));
            await _embyRepository.MarkDeviceAsDeleted(removedDevices);
        }


        #endregion

        public void Dispose()
        {
            _embyClient?.Dispose();
        }
    }
}
