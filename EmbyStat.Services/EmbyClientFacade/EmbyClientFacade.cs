using System;
using System.Threading.Tasks;
using EmbyStat.Common.Exceptions;
using EmbyStat.Services.Emby;
using EmbyStat.Services.Emby.Models;
using MediaBrowser.Model.Users;

namespace EmbyStat.Services.EmbyClientFacade
{
	public class EmbyClientFacade : IEmbyClientFacade
	{
		public async Task<AuthenticationResult> AuthenticateUserAsync(EmbyLogin login)
		{
			using (var client = Client.GetApiClient(login.Address))
			{
				return await client.AuthenticateUserAsync(login.UserName, login.Password);
			}
		}
	}
}
