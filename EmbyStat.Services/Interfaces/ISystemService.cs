using System.Threading.Tasks;

namespace EmbyStat.Services.Interfaces;

public interface ISystemService
{
    Task ResetEmbyStatTables();
}