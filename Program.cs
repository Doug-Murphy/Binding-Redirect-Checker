using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
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

            var serializer = new XmlSerializer(typeof(Configuration));

            var readerWithExplicit = new StreamReader(configFilePathWithExplicitRedirects);
            var deserializedWithExplicit = (Configuration) serializer.Deserialize(readerWithExplicit);
            readerWithExplicit.Close();

            var readerWithoutExplicit = new StreamReader(configFilePathWithoutExplicitRedirects);
            var deserializedWithoutExplicit = (Configuration) serializer.Deserialize(readerWithoutExplicit);
            readerWithoutExplicit.Close();

            var bindingsWithExplicitRedirects = new Dictionary<string, BindingRedirectInfo>();
            var bindingsWithoutExplicitRedirects = new Dictionary<string, BindingRedirectInfo>();

            foreach (AssemblyBinding deserializedRedirect in deserializedWithExplicit.Runtime.AssemblyBinding.OrderBy(x => x.DependentAssembly.AssemblyIdentity.Name)) {
                bindingsWithExplicitRedirects.Add(deserializedRedirect.DependentAssembly.AssemblyIdentity.Name, new BindingRedirectInfo {OldVersion = deserializedRedirect.DependentAssembly.BindingRedirect.OldVersion, NewVersion = deserializedRedirect.DependentAssembly.BindingRedirect.NewVersion});
            }

            foreach (AssemblyBinding deserializedRedirect in deserializedWithoutExplicit.Runtime.AssemblyBinding.OrderBy(x => x.DependentAssembly.AssemblyIdentity.Name)) {
                bindingsWithoutExplicitRedirects.Add(deserializedRedirect.DependentAssembly.AssemblyIdentity.Name, new BindingRedirectInfo {OldVersion = deserializedRedirect.DependentAssembly.BindingRedirect.OldVersion, NewVersion = deserializedRedirect.DependentAssembly.BindingRedirect.NewVersion});
            }

            if (bindingsWithExplicitRedirects.Count > bindingsWithoutExplicitRedirects.Count) {
                foreach (var bindingWithExplicitRedirect in bindingsWithExplicitRedirects) {
                    var matchingBindingWithoutExplicitRedirect = bindingsWithoutExplicitRedirects.FirstOrDefault(x => x.Key == bindingWithExplicitRedirect.Key).Value;

                    if (matchingBindingWithoutExplicitRedirect == null) {
                        Console.WriteLine($"A binding redirect for {bindingWithExplicitRedirect.Key} was only found when explicitly set. This means you shouldn't need it, but you should confirm that with runtime checking.");
                        continue;
                    }
                    
                    if (matchingBindingWithoutExplicitRedirect != bindingWithExplicitRedirect.Value) {
                        Console.WriteLine($"Binding redirect for {bindingWithExplicitRedirect.Key} is different.");
                        Console.WriteLine($"With explicit redirect: OldVersion={bindingWithExplicitRedirect.Value.OldVersion}, NewVersion={bindingWithExplicitRedirect.Value.NewVersion}");
                        Console.WriteLine($"Without explicit redirect: OldVersion={matchingBindingWithoutExplicitRedirect.OldVersion}, NewVersion={matchingBindingWithoutExplicitRedirect.NewVersion}");
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
                        Console.WriteLine($"Binding redirect for {bindingWithoutExplicitRedirect.Key} is different.");
                        Console.WriteLine($"With explicit redirect: OldVersion={matchingBindingWithExplicitRedirect.OldVersion}, NewVersion={matchingBindingWithExplicitRedirect.NewVersion}");
                        Console.WriteLine($"Without explicit redirect: OldVersion={bindingWithoutExplicitRedirect.Value.OldVersion}, NewVersion={bindingWithoutExplicitRedirect.Value.NewVersion}");
                    }
                }
            }

            Console.WriteLine("Finished");
        }
    }
}