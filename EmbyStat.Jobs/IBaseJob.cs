using System;
using System.Threading.Tasks;

namespace EmbyStat.Jobs
{
    public interface IBaseJob : IDisposable
    {
        Task Execute();
    }
}