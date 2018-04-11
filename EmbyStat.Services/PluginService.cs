using System.Collections.Generic;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using MediaBrowser.Model.Plugins;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Services
{
    public class PluginService : IPluginService
    {
	    private readonly ILogger<PluginService> _logger;
	    private readonly IPluginRepository _embyPluginRepository;

	    public PluginService(ILogger<PluginService> logger, IPluginRepository embyPluginRepository)
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
