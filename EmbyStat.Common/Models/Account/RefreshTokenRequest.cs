using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Models.Account
{
    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
