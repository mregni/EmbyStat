using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using EmbyStat.Core.EnvironmentInfo;
using EmbyStat.Core.Processes.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Core.Processes
{
    public class ProcessProvider : IProcessProvider
    {
        private readonly ILogger<ProcessProvider> _logger;

        public ProcessProvider(ILogger<ProcessProvider> logger)
        {
            _logger = logger;
        }

        public ProcessOutput StartAndCapture(string path, string args = null,
            StringDictionary environmentVariables = null)
        {
            var output = new ProcessOutput();
            var process = Start(path,
                args,
                environmentVariables,
                s => output.Lines.Add(new ProcessOutputLine(ProcessOutputLevel.Standard, s)),
                error => output.Lines.Add(new ProcessOutputLine(ProcessOutputLevel.Error, error)));

            process.WaitForExit();
            output.ExitCode = process.ExitCode;

            return output;
        }

        private Process Start(string path, string args = null, StringDictionary environmentVariables = null,
            Action<string> onOutputDataReceived = null, Action<string> onErrorDataReceived = null)
        {
            (path, args) = GetPathAndArgs(path, args);

            var startInfo = new ProcessStartInfo(path, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            if (environmentVariables != null)
            {
                foreach (DictionaryEntry environmentVariable in environmentVariables)
                {
                    try
                    {
                        _logger.LogTrace("Setting environment variable '{environmentVariable.Key}' to '{environmentVariable.Value}'", environmentVariable.Key,
                            environmentVariable.Value);

                        var key = environmentVariable.Key.ToString();
                        var value = environmentVariable.Value?.ToString();

                        if (startInfo.EnvironmentVariables.ContainsKey(key))
                        {
                            startInfo.EnvironmentVariables[key] = value;
                        }
                        else
                        {
                            startInfo.EnvironmentVariables.Add(key, value);
                        }
                    }
                    catch (Exception e)
                    {
                        if (environmentVariable.Value == null)
                        {
                            _logger.LogError(e, "Unable to set environment variable '{0}', value is null",
                                environmentVariable.Key);
                        }
                        else
                        {
                            _logger.LogError(e, "Unable to set environment variable '{0}'", environmentVariable.Key);
                        }

                        throw;
                    }
                }
            }

            _logger.LogDebug("Starting {0} {1}", path, args);

            var process = new Process
            {
                StartInfo = startInfo
            };

            process.OutputDataReceived += (sender, eventArgs) =>
            {
                if (string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    return;
                }

                _logger.LogDebug(eventArgs.Data);

                if (onOutputDataReceived != null)
                {
                    onOutputDataReceived(eventArgs.Data);
                }
            };

            process.ErrorDataReceived += (sender, eventArgs) =>
            {
                if (string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    return;
                }

                _logger.LogDebug(eventArgs.Data);

                if (onErrorDataReceived != null)
                {
                    onErrorDataReceived(eventArgs.Data);
                }
            };

            process.Start();

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            return process;
        }
        
        private (string Path, string Args) GetPathAndArgs(string path, string args)
        {
            if (OsInfo.IsWindows && path.EndsWith(".bat", StringComparison.InvariantCultureIgnoreCase))
            {
                return ("cmd.exe", $"/c {path} {args}");
            }

            if (OsInfo.IsWindows && path.EndsWith(".ps1", StringComparison.InvariantCultureIgnoreCase))
            {
                return ("powershell.exe", $"-ExecutionPolicy Bypass -NoProfile -File {path} {args}");
            }

            if (OsInfo.IsWindows && path.EndsWith(".py", StringComparison.InvariantCultureIgnoreCase))
            {
                return ("python.exe", $"{path} {args}");
            }

            return (path, args);
        }
    }
}