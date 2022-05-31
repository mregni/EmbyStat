using System.Collections.Specialized;

namespace EmbyStat.Core.Processes.Interfaces;

public interface IProcessProvider
{ 
    ProcessOutput StartAndCapture(string path, string args = null, StringDictionary environmentVariables = null);
}