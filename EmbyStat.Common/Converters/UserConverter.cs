using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Logging;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Common.Converters
{
    public static class UserConverter
    {
        public static List<EmbyUser> ConvertToUserList(this JArray model)
        {
            var logger = LogFactory.CreateLoggerForType(typeof(UserConverter), "CONVERTER");
            return model.Children().Select(user =>
            {
                logger.Debug(user.ToString());
                var embyUser = new EmbyUser
                {
                    IsAdministrator = user["Policy"]["IsAdministrator"].Value<bool>(),
                    Id = user["Id"].Value<string>(),
                    Name = user["Name"].Value<string>(),
                    ServerId = user["ServerId"].Value<string>(),
                    Deleted = false
                };
                return embyUser;
            }).ToList();
        }
    }
}