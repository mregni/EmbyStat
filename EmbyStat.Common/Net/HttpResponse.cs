using System.Collections.Generic;
using System.IO;
using System.Net;

namespace EmbyStat.Common.Net
{
    public class HttpResponse
    {
        public string ContentType { get; set; }
        public string ResponseUrl { get; set; }
        public Stream Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public long? ContentLength { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
