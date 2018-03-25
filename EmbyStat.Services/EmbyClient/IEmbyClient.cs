using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Services.Emby.Models;
using MediaBrowser.Model.Users;

namespace EmbyStat.Services.EmbyClient
{
    public interface IEmbyClient : IDisposable
	{
	    Task<AuthenticationResult> AuthenticateUserAsync(string username, string password, string address);
    }
}
