using System;
using System.Collections.Generic;
using System.Text;
using MediaBrowser.Model.Dto;

namespace EmbyStat.Common.Net
{
    public class AuthenticationResult
    {
        public UserDto User { get; set; }
        public SessionInfoDto SessionInfo { get; set; }
        public string AccessToken { get; set; }
        public string ServerId { get; set; }
    }
}
