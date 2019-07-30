using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Migrator.Interfaces
{
    public interface IMigration
    {
        void MigrateUp(ISettingsService settingsService, long version);
    }
}
