using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EmbyStat.Repositories.Config;
using EmbyStat.Services.Config.Models;
using Newtonsoft.Json;

namespace EmbyStat.Services.Config
{
    public class ConfigurationService : IConfigurationService
    {
		private readonly IConfigurationRepository _configurationRepository;
		public ConfigurationService(IConfigurationRepository configurationRepository)
		{
			_configurationRepository = configurationRepository;

		}

		public void SaveServerSettings(Configuration configuration)
		{
			var dbSettings = _configurationRepository.GetSingle();

			dbSettings.Language = configuration.Language;

			_configurationRepository.Update(dbSettings);
		}

		public void SaveEmbySettings(EmbySettings configuration)
		{
			var dbSettings = _configurationRepository.GetSingle();

			dbSettings.EmbyServerAddress = configuration.Address;
			dbSettings.EmbyUserName = configuration.UserName;

			_configurationRepository.Update(dbSettings);

			if (!string.IsNullOrEmpty(configuration.Password))
			{
				using (var client = Emby.Client.GetApiClient(configuration.Address))
				{
					try
					{
						var token = client.AuthenticateUserAsync(configuration.UserName, configuration.Password).Result;
						dbSettings.EmbyServerAddress = token.AccessToken;
						_configurationRepository.Update(dbSettings);
					}
					catch (Exception)
					{

					}

				}
			}
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
						return JsonConvert.DeserializeObject<EmbyUdpBroadcast>(serverResponse);
					}
					catch (Exception)
					{
						// No data recieved, return empty object
					}
				}

				return new EmbyUdpBroadcast();
			}
		}

		public Configuration GetServerSettings()
		{
			return _configurationRepository.GetSingle();
		}
	}
}
