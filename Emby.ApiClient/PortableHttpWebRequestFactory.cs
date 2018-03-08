using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Emby.ApiClient.Net;

namespace Emby.ApiClient
{
    public class PortableHttpWebRequestFactory : IHttpWebRequestFactory
    {
        public HttpWebRequest Create(HttpRequest options)
        {
            var request = HttpWebRequest.CreateHttp(options.Url);

            request.Method = options.Method;

            return request;
        }

        public void SetContentLength(HttpWebRequest request, long length)
        {
            //request.Headers["Content-Length"] = length.ToString(CultureInfo.InvariantCulture);
        }

        public Task<WebResponse> GetResponseAsync(HttpWebRequest request, int timeoutMs)
        {
            if (timeoutMs > 0)
            {
                return GetResponseAsync(request, TimeSpan.FromMilliseconds(timeoutMs));
            }

            var tcs = new TaskCompletionSource<WebResponse>();

            try
            {
                request.BeginGetResponse(iar =>
                {
                    try
                    {
                        var response = (HttpWebResponse)request.EndGetResponse(iar);
                        tcs.SetResult(response);
                    }
                    catch (Exception exc)
                    {
                        tcs.SetException(exc);
                    }
                }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }

            return tcs.Task;
        }

        private Task<WebResponse> GetResponseAsync(WebRequest request, TimeSpan timeout)
        {
            return Task.Factory.StartNew(() =>
            {
                var t = Task.Factory.FromAsync<WebResponse>(
                    request.BeginGetResponse,
                    request.EndGetResponse,
                    null);

                if (!t.Wait(timeout)) throw new TimeoutException();

                return t.Result;
            });
        }

        public Task<Stream> GetRequestStreamAsync(HttpWebRequest request)
        {
            var tcs = new TaskCompletionSource<Stream>();

            try
            {
                request.BeginGetRequestStream(iar =>
                {
                    try
                    {
                        var response = request.EndGetRequestStream(iar);
                        tcs.SetResult(response);
                    }
                    catch (Exception exc)
                    {
                        tcs.SetException(exc);
                    }
                }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }

            return tcs.Task;
        }
    }
}
