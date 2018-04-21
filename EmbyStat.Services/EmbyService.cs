using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models;
using EmbyStat.Services.Models.Emby;
using Newtonsoft.Json;
using Serilog;

namespace EmbyStat.Services
{
    public class EmbyService : IEmbyService
    {
	    private readonly IEmbyClient _embyClient;
	    private readonly IPluginRepository _embyPluginRepository;
	    private readonly IServerInfoRepository _embyServerInfoRepository;
		private readonly IConfigurationRepository _configurationRepository;
		private readonly IDriveRepository _embyDriveRepository;

		public EmbyService(IEmbyClient embyClient, 
						   IPluginRepository embyPluginRepository, 
						   IConfigurationRepository configurationRepository, 
						   IServerInfoRepository embyServerInfoRepository,
						   IDriveRepository embyDriveRepository)
	    {
		    _embyClient = embyClient;
		    _embyPluginRepository = embyPluginRepository;
		    _configurationRepository = configurationRepository;
		    _embyServerInfoRepository = embyServerInfoRepository;
		    _embyDriveRepository = embyDriveRepository;
	    }

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

					    var configuration = _configurationRepository.GetSingle();
					    configuration.ServerName = udpBroadcastResult.Name;
						_configurationRepository.UpdateOrAdd(configuration);

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
					var token = await _embyClient.AuthenticateUserAsync(login.UserName, login.Password, login.Address);
					return new EmbyToken
					{
						Token = token.AccessToken,
						Username = token.User.ConnectUserName,
						IsAdmin = token.User.Policy.IsAdministrator,
                        Id = token.User.Id
					};
				}
				catch (Exception e)
				{
					Log.Error("Username or password are wrong, user should try again with other credentials!");
				    Log.Error($"Message: {e.Message}");
					throw new BusinessException("TOKEN_FAILED");
				}
			}

		    Log.Error("Username or password are empty, no use to try a login!");
			throw new BusinessException("TOKEN_FAILED");
	    }

	    public ServerInfo GetServerInfo()
	    {
		    return _embyServerInfoRepository.GetSingle();
	    }

	    public List<Drives> GetLocalDrives()
	    {
		    return _embyDriveRepository.GetAll();
	    }

	    public async void FireSmallSyncEmbyServerInfo()
	    {
		    var settings = _configurationRepository.GetSingle();

			_embyClient.SetAddressAndUrl(settings.EmbyServerAddress, settings.AccessToken);
		    var systemInfoReponse = await _embyClient.GetServerInfoAsync();
			var pluginsResponse = await _embyClient.GetInstalledPluginsAsync();
		    var drives = await _embyClient.GetLocalDrivesAsync();

		    var systemInfo = Mapper.Map<ServerInfo>(systemInfoReponse);
		    var localDrives = Mapper.Map<IList<Drives>>(drives);

		    _embyServerInfoRepository.UpdateOrAdd(systemInfo);
			_embyPluginRepository.RemoveAllAndInsertPluginRange(pluginsResponse);
			_embyDriveRepository.ClearAndInsertList(localDrives.ToList());
		}
    }
}
