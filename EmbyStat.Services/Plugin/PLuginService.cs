using System.Collections.Generic;
using EmbyStat.Repositories.EmbyPlugin;
using EmbyStat.Services.EmbyClient;
using MediaBrowser.Model.Plugins;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Services.Plugin
{
    public class PluginService : IPluginService
    {
	    private readonly ILogger<PluginService> _logger;
	    private readonly IEmbyClient _embyClient;
	    private readonly IEmbyPluginRepository _embyPluginRepository;

	    public PluginService(ILogger<PluginService> logger, IEmbyClient embyClient, IEmbyPluginRepository embyPluginRepository)
	    {
		    _logger = logger;
		    _embyClient = embyClient;
		    _embyPluginRepository = embyPluginRepository;
	    }

	    public List<PluginInfo> GetInstalledPlugins()
	    {
		    return _embyPluginRepository.GetPlugins();
	    }
	}
}
