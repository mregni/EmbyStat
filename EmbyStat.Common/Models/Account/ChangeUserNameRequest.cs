using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Models.Account
{
    public class ChangeUserNameRequest
    {
        public string UserName { get; set; }
        public string NewUserName { get; set; }
    }
}
