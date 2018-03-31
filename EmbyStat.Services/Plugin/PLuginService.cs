using System.Collections.Generic;
using EmbyStat.Repositories.EmbyPlugin;
using MediaBrowser.Model.Plugins;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Services.Plugin
{
    public class PluginService : IPluginService
    {
	    private readonly ILogger<PluginService> _logger;
	    private readonly IEmbyPluginRepository _embyPluginRepository;

	    public PluginService(ILogger<PluginService> logger, IEmbyPluginRepository embyPluginRepository)
	    {
		    _logger = logger;
		    _embyPluginRepository = embyPluginRepository;
	    }

	    public List<PluginInfo> GetInstalledPlugins()
	    {
		    return _embyPluginRepository.GetPlugins();
	    }
	}
}
