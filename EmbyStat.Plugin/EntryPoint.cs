using EmbyStat.Plugin.Services;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Logging;

namespace EmbyStat.Plugin;

public class EntryPoint : IServerEntryPoint
{
    private readonly ILibraryManager _libraryManager;
    private readonly EmbyStatService _service;

    public EntryPoint(ILogger logger, ILibraryManager libraryManager, IConfigurationManager config)
    {
        _libraryManager = libraryManager;
        _service = new EmbyStatService(config, libraryManager);
    }

    public void Run()
    {
        _libraryManager.ItemAdded += _service.ItemAdded;
        _libraryManager.ItemRemoved += _service.ItemRemoved;
        _libraryManager.ItemUpdated += _service.ItemUpdated;
    }

    public void Dispose()
    {
        _libraryManager.ItemAdded -= _service.ItemAdded;
        _libraryManager.ItemRemoved -= _service.ItemRemoved;
        _libraryManager.ItemUpdated -= _service.ItemUpdated;
    }
}