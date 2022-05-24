using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace EmbyStat.Common.Processes.Interfaces;

public interface IProcessProvider
{ 
    ProcessOutput StartAndCapture(string path, string args = null, StringDictionary environmentVariables = null);
}