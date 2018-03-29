using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.ApiClient;

namespace EmbyStat.Services.EmbyClient.Net
{
    public interface IAsyncHttpClient
    {
	    Task<Stream> SendAsync(HttpRequest options);
	    Task<HttpResponse> GetResponse(HttpRequest options, bool sendFailureResponse = false);
	}
}
