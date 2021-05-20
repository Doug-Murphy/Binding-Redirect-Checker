using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BindingRedirectChecker.Models;

namespace BindingRedirectChecker {
    internal class Program {
        private static void Main(string[] args) {
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
                throw new Exception("You must specify both file paths.");
            }

            configFilePathWithExplicitRedirects = configFilePathWithExplicitRedirects.Trim('"');
            configFilePathWithoutExplicitRedirects = configFilePathWithoutExplicitRedirects.Trim('"');

            var bindingsWithExplicitRedirects = DeserializeConfigFileAndBuildDictionary(configFilePathWithExplicitRedirects);
            var bindingsWithoutExplicitRedirects = DeserializeConfigFileAndBuildDictionary(configFilePathWithoutExplicitRedirects);

            if (bindingsWithExplicitRedirects.Count > bindingsWithoutExplicitRedirects.Count) {
                foreach (var bindingWithExplicitRedirect in bindingsWithExplicitRedirects) {
                    var matchingBindingWithoutExplicitRedirect = bindingsWithoutExplicitRedirects.FirstOrDefault(x => x.Key == bindingWithExplicitRedirect.Key).Value;

                    if (matchingBindingWithoutExplicitRedirect == null) {
                        Console.WriteLine($"A binding redirect for {bindingWithExplicitRedirect.Key} was only found when explicitly set. This means you shouldn't need it, but you should confirm that with runtime checking.");
                        continue;
                    }

                    if (bindingWithExplicitRedirect.Value != matchingBindingWithoutExplicitRedirect) {
                        DisplayDifferencesInBinding(bindingWithExplicitRedirect.Key, bindingWithExplicitRedirect.Value, matchingBindingWithoutExplicitRedirect);
                    }
                }
            }
            else {
                foreach (var bindingWithoutExplicitRedirect in bindingsWithoutExplicitRedirects) {
                    var matchingBindingWithExplicitRedirect = bindingsWithExplicitRedirects.FirstOrDefault(x => x.Key == bindingWithoutExplicitRedirect.Key).Value;

                    if (matchingBindingWithExplicitRedirect == null) {
                        Console.WriteLine($"A binding redirect for {bindingWithoutExplicitRedirect.Key} was only found when not explicitly set. This should be fine because that just means you explicitly set it when you didn't need to, but you should confirm that with runtime checking.");
                        continue;
                    }

                    if (matchingBindingWithExplicitRedirect != bindingWithoutExplicitRedirect.Value) {
                        DisplayDifferencesInBinding(bindingWithoutExplicitRedirect.Key, matchingBindingWithExplicitRedirect, bindingWithoutExplicitRedirect.Value);
                    }
                }
            }

            Console.WriteLine("Finished");
        }

        private static SortedDictionary<string, BindingRedirectInfo> DeserializeConfigFileAndBuildDictionary(string pathToConfigFile) {
            var assembliesWithBindingRedirectInfo = new SortedDictionary<string, BindingRedirectInfo>();

            var doc = XDocument.Load(pathToConfigFile);
            XNamespace ns = "urn:schemas-microsoft-com:asm.v1";
            var assemblyBindingNodes = doc.Descendants(ns + "assemblyBinding");
            foreach (var assemblyBindingNode in assemblyBindingNodes) {
                var dependentAssemblyNodes = assemblyBindingNode.Descendants(ns + "dependentAssembly");
                foreach (var dependentAssemblyNode in dependentAssemblyNodes) {
                    var assemblyIdentityNode = dependentAssemblyNode.Descendants(ns + "assemblyIdentity");
                    var bindingRedirectNode = dependentAssemblyNode.Descendants(ns + "bindingRedirect");

                    var assemblyName = assemblyIdentityNode.Attributes("name").First().Value;
                    var oldVersion = bindingRedirectNode.Attributes("oldVersion").First().Value;
                    var newVersion = bindingRedirectNode.Attributes("newVersion").First().Value;
                    assembliesWithBindingRedirectInfo.Add(assemblyName, new BindingRedirectInfo {OldVersion = oldVersion, NewVersion = newVersion});
                }
            }

            return assembliesWithBindingRedirectInfo;
        }

        private static void DisplayDifferencesInBinding(string assemblyName, BindingRedirectInfo bindingWithExplicitRedirect, BindingRedirectInfo bindingWithoutExplicitRedirect) {
            Console.WriteLine($"Binding redirect for {assemblyName} is different.");
            Console.WriteLine($"With explicit redirect: OldVersion={bindingWithExplicitRedirect.OldVersion}, NewVersion={bindingWithExplicitRedirect.NewVersion}");
            Console.WriteLine($"Without explicit redirect: OldVersion={bindingWithoutExplicitRedirect.OldVersion}, NewVersion={bindingWithoutExplicitRedirect.NewVersion}");
        }
    }
}