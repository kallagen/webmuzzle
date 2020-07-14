using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace TSensor.Proxy
{
    public class Config
    {
        public const string INPUT_MODE_TCP = "tcp";
        public const string INPUT_MODE_COM = "com";

        public const string OUTPUT_MODE_API = "api";
        public const string OUTPUT_MODE_FILE = "file";

        public string DeviceGuid { get; }

        public string LoggerType { get; }
        public int MaxErrorFileSize { get; }

        public string InputMode { get; }
        public IEnumerable<string> COMPortList { get; }
        public IEnumerable<int> TCPPortList { get; }

        public string OutputMode { get; }
        public string ApiUrlSendValue { get; private set; }
        public string ApiUrlSendArchive { get; private set; }

        public string ArchiveFolder { get; private set; }

        public int MaxArchiveFileSize { get; private set; }
        public int ArchiveSendInterval { get; private set; }
        
        public bool IsLinux =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public bool IsTcpInputMode =>
            InputMode == INPUT_MODE_TCP;
        public bool IsComInputMode =>
            InputMode == INPUT_MODE_COM;

        public bool IsApiOutputMode =>
            OutputMode == OUTPUT_MODE_API;
        public bool IsFileOutputMode =>
            OutputMode == OUTPUT_MODE_FILE;

        public Config()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json")
                .Build();

            DeviceGuid = config["deviceGuid"];

            LoggerType = config["loggerType"];
            MaxErrorFileSize = int.Parse(config["maxErrorFileSize"]) * 1024;

            InputMode = config["inputMode"];
            if (IsTcpInputMode)
            {
                TCPPortList = config.GetSection("portList").GetChildren()
                    .SelectMany(p =>
                    {
                        var port = p.Value ?? string.Empty;

                        if (int.TryParse(port, out var _port))
                        {
                            return new[] { _port };
                        }
                        else
                        {
                            var match = Regex.Match(port, "^([0-9]+)\\s?-\\s?([0-9]+)$");
                            if (match.Groups.Count == 3)
                            {
                                var start = int.Parse(match.Groups[1].Value);
                                var end = int.Parse(match.Groups[2].Value);

                                var st = Math.Min(start, end);
                                var count = Math.Max(start, end) - st + 1;

                                return Enumerable.Range(st, count);
                            }
                        }

                        return Enumerable.Empty<int>();
                    }).Distinct().OrderBy(p => p);
            }
            else if (IsComInputMode)
            {
                COMPortList = config.GetSection("portList").GetChildren().Select(p => p.Value?.ToUpper()).Distinct();
            }

            OutputMode = config["outputMode"];
            ApiUrlSendValue = $"http://{config["apiHost"]}/api/sensorvalue/push";
            ApiUrlSendArchive = $"http://{config["apiHost"]}/api/sensorvalue/archive/push";

            ArchiveFolder = config["archiveFolder"];

            MaxArchiveFileSize = int.Parse(config["maxArchiveFileSize"]) * 1024;
            ArchiveSendInterval = int.Parse(config["archiveSendInterval"]) * 1000;
        }
    }
}
