using BindingRedirectChecker.Helpers;
using BindingRedirectChecker.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BindingRedirectChecker {
    internal class Program {
        private static IHostBuilder ConfigureHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddSingleton<ParsingHelper>();
                });
        }

        private static async Task Main(string[] args) {
            string configFilePathWithExplicitRedirects = null;
            string configFilePathWithoutExplicitRedirects = null;

            if (args.Length == 2) {
                configFilePathWithExplicitRedirects = args[0];
                configFilePathWithoutExplicitRedirects = args[1];
            }
            else {
                Console.Write("Enter the full path to the config file that was generated when explicit binding redirects were in place: ");
                configFilePathWithExplicitRedirects = Console.ReadLine();
                Console.Write("Enter the full path to the config file that was generated when explicit binding redirects were removed: ");
                configFilePathWithoutExplicitRedirects = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(configFilePathWithExplicitRedirects) || string.IsNullOrWhiteSpace(configFilePathWithoutExplicitRedirects)) {
                throw new InvalidOperationException("You must specify both file paths.");
            }

            configFilePathWithExplicitRedirects = configFilePathWithExplicitRedirects.Trim('"');
            configFilePathWithoutExplicitRedirects = configFilePathWithoutExplicitRedirects.Trim('"');

            IHost host = ConfigureHostBuilder(args).Build();

            ParsingHelper parsingHelper = host.Services.GetRequiredService<ParsingHelper>();

            var bindingsWithExplicitRedirectsTask = Task.Run(() => parsingHelper.DeserializeConfigFileAndBuildDictionary(configFilePathWithExplicitRedirects));
            var bindingsWithoutExplicitRedirectsTask = Task.Run(() => parsingHelper.DeserializeConfigFileAndBuildDictionary(configFilePathWithoutExplicitRedirects));

            await Task.WhenAll(bindingsWithExplicitRedirectsTask, bindingsWithoutExplicitRedirectsTask);

            SortedDictionary<string, BindingRedirectInfo> bindingsWithExplicitRedirects = await bindingsWithExplicitRedirectsTask;
            SortedDictionary<string, BindingRedirectInfo> bindingsWithoutExplicitRedirects = await bindingsWithoutExplicitRedirectsTask;

            ComparisonResults comparisonResults = parsingHelper.CompareParsedConfigFiles(bindingsWithExplicitRedirects, bindingsWithoutExplicitRedirects);

            Console.WriteLine(comparisonResults);

            Console.WriteLine("Finished");
        }
    }
}