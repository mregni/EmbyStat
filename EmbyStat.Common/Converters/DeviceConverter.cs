using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Common.Converters
{
    public static class DeviceConverter
    {
        public static List<Device> ConvertToDeviceList(this JObject devicesJson)
        {
            return devicesJson["Items"].Children().Select(device => new Device
            {
                Id = device["Id"].Value<string>(),
                Name = device["Name"].Value<string>(),
                Deleted = false,
                AppName = device["AppName"].Value<string>(),
                AppVersion = device["AppVersion"].Value<string>(),
                DateLastActivity = device["DateLastActivity"].Value<DateTime>(),
                IconUrl = device["IconUrl"]?.Value<string>() ?? string.Empty,
                LastUserId = device["LastUserId"].Value<string>()
            }).ToList();
        }
    }
}
