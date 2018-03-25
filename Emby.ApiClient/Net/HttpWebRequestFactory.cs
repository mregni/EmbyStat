using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.ApiClient.Net
{
    public class HttpWebRequestFactory : IHttpWebRequestFactory
    {
        private static PropertyInfo _httpBehaviorPropertyInfo;
        public HttpWebRequest Create(HttpRequest options)
        {
            var request = HttpWebRequest.CreateHttp(options.Url);

            request.AutomaticDecompression = DecompressionMethods.Deflate;
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Revalidate);
            request.KeepAlive = true;
            request.Method = options.Method;
            request.Pipelined = true;
            request.Timeout = options.Timeout;

            // This is a hack to prevent KeepAlive from getting disabled internally by the HttpWebRequest
            /*var sp = request.ServicePoint;
            if (_httpBehaviorPropertyInfo == null)
            {
                _httpBehaviorPropertyInfo = sp.GetType().GetProperty("HttpBehaviour", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            _httpBehaviorPropertyInfo?.SetValue(sp, (byte)0, null);*/

            if (!string.IsNullOrEmpty(options.RequestContent) ||
                string.Equals(options.Method, "post", StringComparison.OrdinalIgnoreCase))
            {
                var bytes = Encoding.UTF8.GetBytes(options.RequestContent ?? string.Empty);

                request.SendChunked = false;
                request.ContentLength = bytes.Length;
            }

            return request;
        }

        public void SetContentLength(HttpWebRequest request, long length)
        {
            request.ContentLength = length;
        }

        public Task<WebResponse> GetResponseAsync(HttpWebRequest request, int timeoutMs)
        {
            if (timeoutMs > 0)
            {
                return GetResponseAsync(request, TimeSpan.FromMilliseconds(timeoutMs));
            }

            return request.GetResponseAsync();
        }

        public Task<Stream> GetRequestStreamAsync(HttpWebRequest request)
        {
            return request.GetRequestStreamAsync();
        }

        private Task<WebResponse> GetResponseAsync(WebRequest request, TimeSpan timeout)
        {
            var taskCompletion = new TaskCompletionSource<WebResponse>();

            Task<WebResponse> asyncTask = Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

            ThreadPool.RegisterWaitForSingleObject((asyncTask as IAsyncResult).AsyncWaitHandle, TimeoutCallback, request, timeout, true);
            asyncTask.ContinueWith(task =>
            {
                taskCompletion.TrySetResult(task.Result);

            }, TaskContinuationOptions.NotOnFaulted);

            // Handle errors
            asyncTask.ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    taskCompletion.TrySetException(task.Exception);
                }
                else
                {
                    taskCompletion.TrySetException(new List<Exception>());
                }

            }, TaskContinuationOptions.OnlyOnFaulted);

            return taskCompletion.Task;
        }

        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                WebRequest request = (WebRequest)state;
                if (state != null)
                {
                    request.Abort();
                }
            }
        }
    }
}
