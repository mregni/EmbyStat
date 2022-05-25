using EmbyStat.Controllers.Settings;

namespace Tests.Unit.Builders.ViewModels;

public class FullSettingsViewModelBuilder
{
    private readonly FullSettingsViewModel _model;

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
                Address = settings.MediaServer.Address,
                AuthorizationScheme = settings.MediaServer.AuthorizationScheme,
                ApiKey = settings.MediaServer.ApiKey,
            }
        };
    }

    public FullSettingsViewModel Build()
    {
        return _model;
    }
}