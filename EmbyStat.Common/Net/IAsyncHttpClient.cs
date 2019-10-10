using System;
using System.IO;
using System.Threading.Tasks;

namespace EmbyStat.Common.Net
{
    public interface IAsyncHttpClient :IDisposable
    {
	    Task<Stream> SendAsync(HttpRequest options);
	    Task<HttpResponse> GetResponse(HttpRequest options, bool sendFailureResponse = false);
	}
}
