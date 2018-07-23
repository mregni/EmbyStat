using System.Collections.Generic;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IConfigurationRepository
    {
        Configuration GetConfiguration();
        void Update(Configuration config);
    }
}
