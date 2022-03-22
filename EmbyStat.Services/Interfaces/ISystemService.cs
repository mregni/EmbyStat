using System.Threading.Tasks;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Services.Interfaces;

public interface ISystemService
{
    Task ResetEmbyStatTables();
}