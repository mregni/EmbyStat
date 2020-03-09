using System.Net.Security;
using EmbyStat.Common.Net;
using NLog;
using RestSharp;

namespace EmbyStat.Common.Extensions
{
    public static class RestClientExtension
    {
        public static IRestClient Initialize(this IRestClient client)
        {
            var logger = LogManager.GetCurrentClassLogger();

            client.UseSerializer(() => new JsonNetSerializer());
            client.UserAgent = "EmbyStat/1.0";
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors != SslPolicyErrors.None)
                {
                    logger.Info($"SSL certificate error found => {sslPolicyErrors.ToString()} found. We are ignoring this error but good idea to fix this problem.");
                }

                return true;
            };

            return client;
        }
    }
}
