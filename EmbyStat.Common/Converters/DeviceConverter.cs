using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models.Entities;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Common.Converters
{
    public static class DeviceConverter
    {
        public static IEnumerable<Device> ConvertToDeviceList(JObject devicesJson)
        {
            foreach (var device in devicesJson["Items"].Children())
            {
                yield return new Device
                {
                    Id = device["Id"].Value<string>(),
                    Name = device["Name"].Value<string>(),
                    Deleted = false,
                    AppName = device["AppName"].Value<string>(),
                    AppVersion = device["AppVersion"].Value<string>(),
                    DateLastActivity = device["DateLastActivity"].Value<DateTime>(),
                    IconUrl = device["IconUrl"]?.Value<string>() ?? string.Empty,
                    LastUserId = device["LastUserId"].Value<string>()
                };
            }
        }
    }
}
