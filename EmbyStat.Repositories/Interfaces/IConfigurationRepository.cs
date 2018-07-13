using System.Collections.Generic;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IConfigurationRepository
    {
        Dictionary<string, string> GetConfiguration();
	    void UpdateOrAdd(Dictionary<string, string> entities);
	}
}
