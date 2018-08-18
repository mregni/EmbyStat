using System.Collections.Generic;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IConfigurationRepository
    {
        Configuration GetConfiguration();
        void Update(Configuration config);
    }
}
