using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Net;

namespace EmbyStat.Common.Net
{
	public class HttpWebRequestClient : IAsyncHttpClient
	{
		private readonly IHttpWebRequestFactory _requestFactory;

		public HttpWebRequestClient(IHttpWebRequestFactory requestFactory)
		{
			_requestFactory = requestFactory;
		}

		public async Task<HttpResponse> GetResponse(HttpRequest options, bool sendFailureResponse = false)
		{
			options.CancellationToken.ThrowIfCancellationRequested();
			var httpWebRequest = _requestFactory.Create(options);
			ApplyHeaders(options.RequestHeaders, httpWebRequest);

			if (options.RequestStream != null)
			{
				httpWebRequest.ContentType = options.RequestContentType;
				_requestFactory.SetContentLength(httpWebRequest, options.RequestStream.Length);

				using (var requestStream = await _requestFactory.GetRequestStreamAsync(httpWebRequest).ConfigureAwait(false))
				{
					await options.RequestStream.CopyToAsync(requestStream).ConfigureAwait(false);
				}
			}
			else if (!string.IsNullOrEmpty(options.RequestContent) || string.Equals(options.Method, "post", StringComparison.OrdinalIgnoreCase))
			{
				var bytes = Encoding.UTF8.GetBytes(options.RequestContent ?? string.Empty);

				httpWebRequest.ContentType = options.RequestContentType ?? "application/x-www-form-urlencoded";
				_requestFactory.SetContentLength(httpWebRequest, bytes.Length);

				using (var requestStream = await _requestFactory.GetRequestStreamAsync(httpWebRequest).ConfigureAwait(false))
				{
					await requestStream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
				}
			}

			try
			{
				options.CancellationToken.ThrowIfCancellationRequested();
				var response = await _requestFactory.GetResponseAsync(httpWebRequest, options.Timeout).ConfigureAwait(false);
				var httpResponse = (HttpWebResponse)response;
				var headers = ConvertHeaders(response);

				EnsureSuccessStatusCode(httpResponse);
				options.CancellationToken.ThrowIfCancellationRequested();

				return GetResponse(httpResponse, headers);
			}
			catch (OperationCanceledException ex)
			{
				var exception = GetCancellationException(options.Url, options.CancellationToken, ex);
				throw exception;
			}
			catch (Exception ex)
			{
				if (sendFailureResponse)
				{
					var webException = ex as WebException ?? ex.InnerException as WebException;
					if (webException?.Response is HttpWebResponse response)
					{
						var headers = ConvertHeaders(response);
						return GetResponse(response, headers);
					}
				}

				throw GetExceptionToThrow(ex);
			}
		}

		private HttpResponse GetResponse(HttpWebResponse httpResponse, Dictionary<string, string> headers)
		{
			return new HttpResponse
			{
				Content = httpResponse.GetResponseStream(),
                StatusCode = httpResponse.StatusCode,
                ContentType = httpResponse.ContentType,
                Headers = headers,
                ContentLength = GetContentLength(httpResponse),
                ResponseUrl = httpResponse.ResponseUri.ToString()
			};
		}

		private long? GetContentLength(HttpWebResponse response)
		{
			var length = response.ContentLength;

			if (length == 0)
			{
				return null;
			}

			return length;
		}

		public async Task<Stream> SendAsync(HttpRequest options)
		{
			var response = await GetResponse(options).ConfigureAwait(false);

			return response.Content;
		}

		private Dictionary<string, string> ConvertHeaders(WebResponse response)
		{
			var headers = response.Headers;

			return headers.Cast<string>().ToDictionary(p => p, p => headers[p]);
		}

		private Exception GetExceptionToThrow(Exception ex)
		{
			var webException = ex as WebException ?? ex.InnerException as WebException;

			if (webException != null)
			{
				var httpException = new HttpException(ex.Message, ex);

				if (webException.Response is HttpWebResponse response)
				{
					httpException.StatusCode = response.StatusCode;
				}

				return httpException;
			}

			var timeoutException = ex as TimeoutException ?? ex.InnerException as TimeoutException;
			if (timeoutException != null)
			{
				var httpException = new HttpException(ex.Message, ex)
				{
					IsTimedOut = true
				};

				return httpException;
			}

			return ex;
		}

		private void ApplyHeaders(HttpHeaders headers, HttpWebRequest request)
		{
			foreach (var header in headers)
			{
				request.Headers[header.Key] = header.Value;
			}

			if (!string.IsNullOrEmpty(headers.AuthorizationScheme))
			{
				var val = string.Format("{0} {1}", headers.AuthorizationScheme, headers.AuthorizationParameter);
				request.Headers["X-Emby-Authorization"] = val;
			}
		}

		private Exception GetCancellationException(string url, CancellationToken cancellationToken, OperationCanceledException exception)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				var msg = string.Format("Connection to {0} timed out", url);

				return new HttpException(msg, exception)
				{
					IsTimedOut = true
				};
			}

			return exception;
		}

		private void EnsureSuccessStatusCode(HttpWebResponse response)
		{
			var statusCode = response.StatusCode;
			var isSuccessful = statusCode >= HttpStatusCode.OK && statusCode <= (HttpStatusCode)299;

			if (!isSuccessful)
			{
				throw new HttpException(response.StatusDescription) { StatusCode = response.StatusCode };
			}
		}

		public void Dispose()
		{
		}
	}

	public interface IHttpWebRequestFactory
	{
		HttpWebRequest Create(HttpRequest options);
		void SetContentLength(HttpWebRequest request, long length);
		Task<WebResponse> GetResponseAsync(HttpWebRequest request, int timeoutMs);
		Task<Stream> GetRequestStreamAsync(HttpWebRequest request);
	}
}
