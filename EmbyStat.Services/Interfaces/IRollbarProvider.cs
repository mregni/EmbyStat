using System;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Settings;

namespace EmbyStat.Services.Interfaces;

public interface ISettingsService
{
    void CreateRollbarLogger();
}