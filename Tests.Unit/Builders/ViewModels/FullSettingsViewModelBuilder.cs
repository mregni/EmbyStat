using System.Linq;
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
                MovieLibraries = settings.MovieLibraries.ToList(),
                ShowLibraries = settings.ShowLibraries.ToList(),
                ToShortMovie = settings.ToShortMovie,
                ToShortMovieEnabled = settings.ToShortMovieEnabled,
                UpdateInProgress = settings.UpdateInProgress,
                UpdateTrain = (int) settings.UpdateTrain,
                Username = settings.Username,
                WizardFinished = settings.WizardFinished,
                MediaServer = new MediaServerSettingsViewModel
                {
                    ServerAddress = settings.MediaServer.ServerAddress,
                    AuthorizationScheme = settings.MediaServer.AuthorizationScheme,
                    ServerPort = settings.MediaServer.ServerPort,
                    ApiKey = settings.MediaServer.ApiKey,
                    ServerName = settings.MediaServer.ServerName,
                    ServerProtocol = (int) settings.MediaServer.ServerProtocol
                }
            };
        }
        public FullSettingsViewModel Build()
        {
            return _model;
        }
    }
}
