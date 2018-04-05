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
using EmbyStat.Repositories.Config;
using EmbyStat.Repositories.EmbyDrive;
using EmbyStat.Repositories.EmbyPlugin;
using EmbyStat.Repositories.EmbyServerInfo;
using EmbyStat.Services.Emby.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmbyStat.Services.Emby
{
    public class EmbyService : IEmbyService
    {
	    private readonly ILogger<EmbyService> _logger;
	    private readonly IEmbyClient _embyClient;
	    private readonly IEmbyPluginRepository _embyPluginRepository;
	    private readonly IEmbyServerInfoRepository _embyServerInfoRepository;
		private readonly IConfigurationRepository _configurationRepository;
		private readonly IEmbyDriveRepository _embyDriveRepository;

		public EmbyService(ILogger<EmbyService> logger, 
						   IEmbyClient embyClient, 
						   IEmbyPluginRepository embyPluginRepository, 
						   IConfigurationRepository configurationRepository, 
						   IEmbyServerInfoRepository embyServerInfoRepository,
						   IEmbyDriveRepository embyDriveRepository)
	    {
		    _logger = logger;
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
						IsAdmin = token.User.Policy.IsAdministrator
					};
				}
				catch (Exception e)
				{
					_logger.LogError("Username or password are wrong, user should try again with other credentials!");
					_logger.LogError($"Message: {e.Message}");
					throw new BusinessException("TOKEN_FAILED");
				}
			}
			
			_logger.LogError("Username or password are empty, no use to try a login!");
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
		    var systemInfoReponse = await _embyClient.GetServerInfo();
			var pluginsResponse = await _embyClient.GetInstalledPluginsAsync();
		    var drives = await _embyClient.GetLocalDrives();

		    var systemInfo = Mapper.Map<ServerInfo>(systemInfoReponse);
		    var localDrives = Mapper.Map<IList<Drives>>(drives);

		    _embyServerInfoRepository.UpdateOrAdd(systemInfo);
			_embyPluginRepository.RemoveAllAndInsertPluginRange(pluginsResponse);
			_embyDriveRepository.ClearAndInsertList(localDrives.ToList());
		}
    }
}
