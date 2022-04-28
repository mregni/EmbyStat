using EmbyStat.Plugin.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Library;

namespace EmbyStat.Plugin.Services;

public class EmbyStatService
{
    private readonly IConfigurationManager _configurationManager;
    private readonly ILibraryManager _libraryManager;

    public EmbyStatService(IConfigurationManager configurationManager, ILibraryManager libraryManager)
    {
        _configurationManager = configurationManager;
        _libraryManager = libraryManager;
    }

    public void ItemUpdated(object sender, ItemChangeEventArgs e)
    {
        AddItemLibraryToNextScan(e);
    }

    public void ItemRemoved(object sender, ItemChangeEventArgs e)
    {
        AddItemLibraryToNextScan(e);
    }

    public void ItemAdded(object sender, ItemChangeEventArgs e)
    {
        AddItemLibraryToNextScan(e);
    }

    private void AddItemLibraryToNextScan(ItemChangeEventArgs e)
    {
        if (e.Item.GetType() != typeof(Movie))
        {
            return;
        }

        var library = _libraryManager.GetLibraryOptions(e.Item);
        var configuration = _configurationManager.GetEmbyStatConfiguration();

        if (!configuration.LibraryTypesToScan.Contains(library.ContentType))
        {
            return;
        }

        configuration.LibraryTypesToScan.Add(library.ContentType);
        _configurationManager.SaveEmbyStatConfiguration(configuration);
    }
}