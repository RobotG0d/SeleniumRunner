using Newtonsoft.Json;
using SeleniumRunner.Model;
using SeleniumRunner.Model.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SeleniumRunner.CLI
{
    class Program
    {
        private static T DeserializeJsonFile<T>(string uri)
        {
            using (StreamReader stream = File.OpenText(uri))
            using (JsonReader reader = new JsonTextReader(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        private static ProjectReport ExecuteFile(Runner runner, string path)
        {
            SideFile side = DeserializeJsonFile<SideFile>(path);
            return runner.Run(side);
        }

        private static IEnumerable<ProjectReport> ExecuteDirectoryFiles(Runner runner, string path)
        {
            ConcurrentBag<ProjectReport> reports = new ConcurrentBag<ProjectReport>();

            Parallel.ForEach(Directory.GetFiles(path), filePath => reports.Add(ExecuteFile(runner, filePath)));

            return reports;
        }

        private static void PrintReports(IEnumerable<ProjectReport> reports)
        {
            if (reports == null || !reports.Any())
                return;

            Console.WriteLine("======================== Report Start ========================");

            foreach (ProjectReport projectReport in reports)
            {
                Console.WriteLine($"Project {projectReport.Project.Name} - START");

                foreach (TestReport testReport in projectReport.TestReports)
                    Console.WriteLine($"Test {testReport.Test.Name} took {testReport.TimeSpan.Milliseconds}ms and was a {(testReport.Success ? "success" : "failure")}.");

                Console.WriteLine($"Project {projectReport.Project.Name} - END");
            }

            Console.WriteLine("========================= Report End =========================");
        }

        static void Main(string[] args)
        {
            Runner runner = new Runner(new CLIRunnerListener());

            ConcurrentBag<ProjectReport> fileReports = new ConcurrentBag<ProjectReport>();
            ConcurrentBag<IEnumerable<ProjectReport>> directoryReports = new ConcurrentBag<IEnumerable<ProjectReport>>();

            Parallel.ForEach(args, uri =>
            {
                if (File.Exists(uri))
                    fileReports.Add(ExecuteFile(runner, uri));
                else if (Directory.Exists(uri))
                    directoryReports.Add(ExecuteDirectoryFiles(runner, uri));
                else
                    Console.WriteLine($"Path {uri} is not a valid file nor directory.");
            });

            directoryReports.Add(fileReports);

            PrintReports(directoryReports.SelectMany(r => r));

            Console.WriteLine("Press enter to exit...");
            Console.ReadKey();
        }
    }
}
