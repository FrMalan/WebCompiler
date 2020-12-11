using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebCompiler
{
    public static class Program
    {
        public static int Main(params string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("\x1B[33mNo arguments");

                return 0;
            }

            var configPath = args[0];
            var file = args.Length > 1 ? args[1] : null;
            var configs = GetConfigs(configPath, file);

            if (configs == null)
            {
                Console.WriteLine("\x1B[33mNo configurations matched");
                return 0;
            }

            var processor = new ConfigFileProcessor();
            EventHookups(processor);

            var results = processor.Process(configPath, configs);
            var errorResults = results.Where(r => r.HasErrors).ToList();

            foreach (var error in errorResults.SelectMany(result => result.Errors))
            {
                Console.Write("\x1B[31m" + error.Message);
            }

            return errorResults.Any() ? 1 : 0;
        }

        private static void EventHookups(ConfigFileProcessor processor)
        {
            // For console colors, see http://stackoverflow.com/questions/23975735/what-is-this-u001b9-syntax-of-choosing-what-color-text-appears-on-console

            processor.BeforeProcess += (s, e) => { Console.WriteLine($"Processing \x1B[36m{e.Config.InputFile}"); if (e.ContainsChanges) FileHelpers.RemoveReadonlyFlagFromFile(e.Config.GetAbsoluteOutputFile()); };
            processor.AfterProcess += (s, e) => { Console.WriteLine("  \x1B[32mCompiled"); };
            processor.BeforeWritingSourceMap += (s, e) => { if (e.ContainsChanges) FileHelpers.RemoveReadonlyFlagFromFile(e.ResultFile); };
            processor.AfterWritingSourceMap += (s, e) => { Console.WriteLine("  \x1B[32mSourcemap"); };
            processor.ConfigProcessed += (s, e) => { Console.WriteLine("\t"); };

            FileMinifier.BeforeWritingMinFile += (s, e) => { FileHelpers.RemoveReadonlyFlagFromFile(e.ResultFile); };
            FileMinifier.AfterWritingMinFile += (s, e) => { Console.WriteLine("  \x1B[32mMinified"); };
            FileMinifier.BeforeWritingGzipFile += (s, e) => { FileHelpers.RemoveReadonlyFlagFromFile(e.ResultFile); };
            FileMinifier.AfterWritingGzipFile += (s, e) => { Console.WriteLine("  \x1B[32mGZipped"); };
        }

        private static IEnumerable<Config> GetConfigs(string configPath, string file)
        {
            var configs = ConfigHandler.GetConfigs(configPath).ToList();

            if (!configs.Any())
            {
                return null;
            }

            if (file != null)
            {
                configs = new List<Config>(file.StartsWith("*")
                    ? configs.Where(c => Path.GetExtension(c.InputFile).Equals(file.Substring(1), StringComparison.OrdinalIgnoreCase))
                    : configs.Where(c => c.InputFile.Equals(file, StringComparison.OrdinalIgnoreCase))
                );
            }

            return configs;
        }
    }
}
