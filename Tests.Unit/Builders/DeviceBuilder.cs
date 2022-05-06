using System;
using EmbyStat.Common.Models.Entities;

namespace Tests.Unit.Builders;

public class DeviceBuilder
{
    private readonly Device _device;

    public DeviceBuilder(string id)
    {
        _device = new Device
        {
            Id = id,
            Name = "deviceName",
            DateLastActivity = DateTime.Today.AddDays(-2)
        };
    }

    public DeviceBuilder AddLastActivityDate(DateTime date)
    {
        _device.DateLastActivity = date;
        return this;
    }

    public Device Build()
    {
        return _device;
    }
}