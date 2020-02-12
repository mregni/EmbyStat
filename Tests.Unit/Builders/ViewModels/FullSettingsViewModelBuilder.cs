using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Controllers.Settings;
using static EmbyStat.Controllers.Settings.FullSettingsViewModel;

namespace Tests.Unit.Builders.ViewModels
{
    public class FullSettingsViewModelBuilder
    {
        private FullSettingsViewModel _model;

        public FullSettingsViewModelBuilder(UserSettings settings)
        {
            _model = new FullSettingsViewModel
            {
                AppName = settings.AppName,
                AutoUpdate = settings.AutoUpdate,
                EnableRollbarLogging = settings.EnableRollbarLogging,
                Id = settings.Id,
                KeepLogsCount = settings.KeepLogsCount,
                Language = settings.Language,
                MovieLibraryTypes = settings.MovieLibraryTypes.Select(x => (int) x).ToList(),
                ShowLibraryTypes = settings.ShowLibraryTypes.Select(x => (int) x).ToList(),
                ToShortMovie = settings.ToShortMovie,
                ToShortMovieEnabled = settings.ToShortMovieEnabled,
                UpdateInProgress = settings.UpdateInProgress,
                UpdateTrain = (int) settings.UpdateTrain,
                Username = settings.Username,
                WizardFinished = settings.WizardFinished,
                Emby = new EmbySettingsViewModel
                {
                    ServerAddress = settings.Emby.ServerAddress,
                    AuthorizationScheme = settings.Emby.AuthorizationScheme,
                    ServerPort = settings.Emby.ServerPort,
                    ApiKey = settings.Emby.ApiKey,
                    ServerName = settings.Emby.ServerName,
                    ServerProtocol = (int) settings.Emby.ServerProtocol
                }
            };
        }
        public FullSettingsViewModel Build()
        {
            return _model;
        }
    }
}
