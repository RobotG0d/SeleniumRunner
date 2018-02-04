using Newtonsoft.Json;
using SeleniumRunner.Model;
using SeleniumRunner.Model.Entities;
using System;
using System.IO;

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

        static void Main(string[] args)
        {
            foreach (string uri in args)
            {
                SideFile side = DeserializeJsonFile<SideFile>(uri);
                new Runner(new CLIRunnerListener()).Run(side);
            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadKey();
        }
    }
}
