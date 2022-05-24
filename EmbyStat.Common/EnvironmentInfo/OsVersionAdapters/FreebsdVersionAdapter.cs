using System;
using System.Linq;
using EmbyStat.Common.Processes.Interfaces;

namespace EmbyStat.Common.EnvironmentInfo.OsVersionAdapters
{
    public class FreebsdVersionAdapter : IOsVersionAdapter
    {
        private readonly IProcessProvider _processProvider;

        public FreebsdVersionAdapter(IProcessProvider processProvider)
        {
            _processProvider = processProvider;
        }

        public OsVersionModel Read()
        {
            if (!OperatingSystem.IsFreeBSD())
            {
                return null;
            }
            
            var output = _processProvider.StartAndCapture("freebsd-version");
            var version = output.Standard.First().Content;

            return new OsVersionModel("FreeBSD", version, $"FreeBSD {version}");

        }

        public bool Enabled => OperatingSystem.IsFreeBSD();
    }
}
