using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Models.Account
{
    public class ChangePasswordRequest
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
