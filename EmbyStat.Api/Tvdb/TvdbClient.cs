using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using EmbyStat.Common;
using MediaBrowser.Common.Net;
using Serilog;

namespace EmbyStat.Api.Tvdb
{
    public class TvdbClient : ITvdbClient
    {
        public async Task<string> GetServerTime(CancellationToken cancellationToken)
        {
            var request = new HttpClient { BaseAddress = new Uri(Constants.Tvdb.BaseUrl) };

            using (var stream = await request.GetStreamAsync(Constants.Tvdb.ServerTimeUrl))
            {
                return GetUpdateTime(stream);
            }
        }

        private string GetUpdateTime(Stream response)
        {
            using (var streamReader = new StreamReader(response, Encoding.UTF8))
            {
                return XElement
                    .Load(streamReader)
                    .Descendants("Time").FirstOrDefault()?.Value;
            }
        }

        private DateTime StringToDateTime(string date)
        {
            if (date.Length == 10)
            {
                var year = int.Parse(date.Substring(0, 4));
                var month = int.Parse(date.Substring(5, 2));
                var day = int.Parse(date.Substring(8, 2));
                return new DateTime(year, month, day);
            }

            return DateTime.MaxValue;
        }

        private string NormalizeLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                return language;

            return language.Split('-')[0].ToLower();
        }

        private void DeleteXmlFiles()
        {
            try
            {
                foreach (var file in Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}\\{Constants.TempFolder}"))
                    File.Delete(file);
            }
            catch (IOException)
            {
                Log.Warning("Could not find a file to delete, maybe it is already gone?");
            }
        }
    }
}
