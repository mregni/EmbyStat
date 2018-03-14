using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Exceptions;
using EmbyStat.Services.Emby.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmbyStat.Services.Emby
{
    public class EmbyService : IEmbyService
    {
	    private readonly ILogger<EmbyService> _logger;

	    public EmbyService(ILogger<EmbyService> logger)
	    {
		    _logger = logger;
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
					    // No data recieved, swallow exception and return empty object
				    }
			    }

			    return new EmbyUdpBroadcast();
		    }
	    }

	    public async Task<EmbyToken> GetEmbyToken(EmbyLogin login)
		{
		    if (!string.IsNullOrEmpty(login.Password) && !string.IsNullOrEmpty(login.UserName))
		    {
			    using (var client = Emby.Client.GetApiClient(login.Address))
			    {
				    try
				    {
					    var token = await client.AuthenticateUserAsync(login.UserName, login.Password);
						return new EmbyToken
						{
							Token = token.AccessToken,
							Username = token.User.ConnectUserName,
							IsAdmin = token.User.Policy.IsAdministrator
						};
				    }
				    catch (Exception)
				    {
					    _logger.LogError("Username or password are wrong, user should try again with other credentials!");
					    throw new BusinessException("WRONG_USERNAME_OR_PASSWORD");
					}

			    }
		    }
			
			_logger.LogError("Username or password are empty, this should not happen!");
			throw new BusinessException("WRONGUSERNAMEORPASSWORD");
	    }
	}
}
