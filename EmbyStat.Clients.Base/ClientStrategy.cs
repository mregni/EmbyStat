using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Base
{
    public class ClientStrategy : IClientStrategy
    {
        private readonly IEnumerable<IClientFactory> _httpFactories;

        public ClientStrategy(IEnumerable<IClientFactory> httpFactories)
        {
            _httpFactories = httpFactories;
        }

        public IHttpClient CreateHttpClient(ServerType type)
        {
            var factory = _httpFactories.FirstOrDefault(x => x.AppliesTo(type));

            if (factory == null)
            {
                throw new TypeLoadException("type not registered");
            }

            return factory.CreateHttpClient();
        }

        public IWebSocketClient CreateWebSocketClient(ServerType type)
        {
            var factory = _httpFactories.FirstOrDefault(x => x.AppliesTo(type));

            if (factory == null)
            {
                throw new TypeLoadException("type not registered");
            }

            return factory.CreateWebSocketClient();
        }
    }
}
