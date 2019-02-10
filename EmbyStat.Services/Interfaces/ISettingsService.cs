using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Settings;

namespace EmbyStat.Services.Interfaces
{
    public interface ISettingsService
    {
        event EventHandler<GenericEventArgs<UserSettings>> OnUserSettingsChanged;
        AppSettings GetAppSettings();
        UserSettings GetUserSettings();
        Task SaveUserSettingsToFile(UserSettings userSettings);
        Task SetUpdateInProgressSetting(bool value);
    }
}
