using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ISqliteBootstrap
    {
        SqliteConnection CreateConnection();
    }
}
