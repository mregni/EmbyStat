

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Exceptions;
using EmbyStat.Services.Emby.Models;
using EmbyStat.Services.EmbyClientFacade;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmbyStat.Services.Emby
{
    public class EmbyService : IEmbyService
    {
	    private readonly ILogger<EmbyService> _logger;
	    private readonly IEmbyClientFacade _embyClientFacade;

	    public EmbyService(ILogger<EmbyService> logger, IEmbyClientFacade embyClientFacade)
	    {
		    _logger = logger;
		    _embyClientFacade = embyClientFacade;
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
		    if (!string.IsNullOrEmpty(login?.Password) && !string.IsNullOrEmpty(login.UserName))
		    {
				try
				{
					var token = await _embyClientFacade.AuthenticateUserAsync(login);
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
					throw new BusinessException("WRONG_USERNAME_OR_PASSWORD");
				}
			}
			
			_logger.LogError("Username or password are empty, no use to try a login!");
			throw new BusinessException("WRONG_USERNAME_OR_PASSWORD");
	    }
	}
}
