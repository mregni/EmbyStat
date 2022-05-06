using System.Threading.Tasks;

namespace EmbyStat.Jobs;

public interface IBaseJob
{
    Task Execute();
}