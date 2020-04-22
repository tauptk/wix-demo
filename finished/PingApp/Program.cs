using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PingApp.Properties;

namespace PingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = Settings.Default.url;

            var pingStartInfo = new ProcessStartInfo();
            pingStartInfo.FileName = "ping";
            pingStartInfo.Arguments = url;
            pingStartInfo.UseShellExecute = false;

            var pingProcess = new Process();
            pingProcess.StartInfo = pingStartInfo;
            pingProcess.Start();
            pingProcess.WaitForExit();

            var exitCode = pingProcess.ExitCode;

            CreateJsonHistoryRecord(url, exitCode);

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        static void CreateJsonHistoryRecord(string url, int exitCode)
        {
            var currentTime = DateTime.Now;

            var jsonHistoryDirectoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PingApp"
            );
            if (!Directory.Exists(jsonHistoryDirectoryPath))
            {
                Directory.CreateDirectory(jsonHistoryDirectoryPath);
            }

            var jsonHistoryFilePath = Path.Combine(jsonHistoryDirectoryPath, "history.json");
            if (!File.Exists(jsonHistoryFilePath))
            {
                File.WriteAllText(jsonHistoryFilePath, "{}");
            }

            Console.WriteLine($"Writing record to {jsonHistoryFilePath}");

            JObject history = JObject.Parse(File.ReadAllText(jsonHistoryFilePath));
            if (!history.ContainsKey(url))
            {
                history.Add(url, new JArray());
            }
            var newEntry = new JObject
            {
                {"datetime", currentTime.ToString()},
                {"exitcode", exitCode}
            };
            var urlArray = history[url] as JArray;
            urlArray?.Add(newEntry);

            File.WriteAllText(jsonHistoryFilePath, history.ToString(Formatting.Indented));
        }
    }
}
