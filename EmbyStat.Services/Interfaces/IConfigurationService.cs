using System.Collections.Generic;
using EmbyStat.Common.Models;

namespace EmbyStat.Services.Interfaces
{
    public interface IConfigurationService
    {
	    void SaveServerSettings(Dictionary<string, string> configuration);
        Dictionary<string, string> GetServerSettings();
	}
}
