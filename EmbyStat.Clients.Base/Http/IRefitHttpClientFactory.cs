using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbyStat.Clients.Base.Http
{
    public interface IRefitHttpClientFactory<T>
    {
        T CreateClient(string baseAddressKey);
    }
}
