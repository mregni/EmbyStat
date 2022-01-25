using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace EmbyStat.Clients.Base.Http
{
    public class RefitHttpClientFactory<T> : IRefitHttpClientFactory<T>
    {
        private readonly IHttpClientFactory _clientFactory;

        public RefitHttpClientFactory(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public T CreateClient(string baseAddressKey)
        {
            var client = _clientFactory.CreateClient("mediaServerClient");
            client.BaseAddress = new Uri(baseAddressKey);

            return RestService.For<T>(client);
        }
    }
}
