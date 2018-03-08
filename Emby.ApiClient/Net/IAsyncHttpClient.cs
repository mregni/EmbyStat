using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Emby.ApiClient.Net
{
    /// <summary>
    /// Interface IHttpClient
    /// </summary>
    public interface IAsyncHttpClient : IDisposable
    {
        /// <summary>
        /// Occurs when [HTTP response received].
        /// </summary>
        event EventHandler<HttpResponseEventArgs> HttpResponseReceived;

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        Task<Stream> SendAsync(HttpRequest options);

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="sendFailureResponse">if set to <c>true</c> [send failure response].</param>
        /// <returns>
        /// Task&lt;HttpResponse&gt;.
        /// </returns>
        Task<HttpResponse> GetResponse(HttpRequest options, bool sendFailureResponse = false);
    }
}
