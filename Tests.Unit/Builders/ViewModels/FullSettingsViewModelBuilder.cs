using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Controllers.Settings;

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
                ToShortMovie = settings.ToShortMovie,
                ToShortMovieEnabled = settings.ToShortMovieEnabled,
                UpdateInProgress = settings.UpdateInProgress,
                UpdateTrain = (int) settings.UpdateTrain,
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

        public FullSettingsViewModelBuilder AddMovieLibraries(IEnumerable<LibraryContainer> libraries)
        {
            _model.MovieLibraries = libraries
                .Select(x => new LibraryContainerViewModel {Id = x.Id, Name = x.Name, LastSynced = x.LastSynced})
                .ToList();

            return this;
        }

        public FullSettingsViewModelBuilder AddShowLibraries(IEnumerable<LibraryContainer> libraries)
        {
            _model.ShowLibraries = libraries
                .Select(x => new LibraryContainerViewModel { Id = x.Id, Name = x.Name, LastSynced = x.LastSynced })
                .ToList();

            return this;
        }

        public FullSettingsViewModel Build()
        {
            return _model;
        }
    }
}
