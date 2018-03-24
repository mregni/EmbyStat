using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Services.Emby.Models;
using MediaBrowser.Model.Users;

namespace EmbyStat.Services.EmbyClientFacade
{
    public interface IEmbyClientFacade
    {
	    Task<AuthenticationResult> AuthenticateUserAsync(EmbyLogin login);

    }
}
