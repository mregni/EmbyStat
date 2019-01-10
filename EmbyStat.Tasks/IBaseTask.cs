using System;
using System.Threading.Tasks;

namespace EmbyStat.Tasks.Tasks
{
    public interface IBaseTask : IDisposable
    {
        string Name { get; }
        string Key { get; }
        string Description { get; }
        string Category { get; }
        Task Execute();
    }
}