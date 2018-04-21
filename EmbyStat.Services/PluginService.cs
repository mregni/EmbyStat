using System.Collections.Generic;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using MediaBrowser.Model.Plugins;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Services
{
    public class PluginService : IPluginService
    {
	    private readonly IPluginRepository _embyPluginRepository;

	    public PluginService(IPluginRepository embyPluginRepository)
	    {
		    _embyPluginRepository = embyPluginRepository;
	    }

	    public List<PluginInfo> GetInstalledPlugins()
	    {
		    return _embyPluginRepository.GetPlugins();
	    }
	}
}
