using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace EmbyStat.Clients.EmbyClient.Net
{
    public class HttpRequest
    {
	    public string Method { get; set; }
	    public CancellationToken CancellationToken { get; set; }
	    public string RequestContent { get; set; }
	    public string RequestContentType { get; set; }
	    public HttpHeaders RequestHeaders { get; set; }
	    public string Url { get; set; }
	    public Stream RequestStream { get; set; }
	    public int Timeout { get; set; }
        public string UserAgent { get; set; }

	    public HttpRequest()
	    {
		    RequestHeaders = new HttpHeaders();
		    Timeout = 120000;
	    }

	    public void SetPostData(IDictionary<string, string> values)
	    {
		    var strings = values.Keys.Select(key => string.Format("{0}={1}", key, values[key]));
		    var postContent = string.Join("&", strings.ToArray());

		    RequestContent = postContent;
		    RequestContentType = "application/x-www-form-urlencoded";
	    }
	}
}
