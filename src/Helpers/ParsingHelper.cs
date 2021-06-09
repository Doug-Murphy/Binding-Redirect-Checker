using BindingRedirectChecker.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BindingRedirectChecker.Helpers {
    public class ParsingHelper {
        public SortedDictionary<string, BindingRedirectInfo> DeserializeConfigFileAndBuildDictionary(string pathToConfigFile) {
            var assembliesWithBindingRedirectInfo = new SortedDictionary<string, BindingRedirectInfo>();

            var doc = XDocument.Load(pathToConfigFile);
            XNamespace ns = "urn:schemas-microsoft-com:asm.v1";
            IEnumerable<XElement> assemblyBindingNodes = doc.Descendants(ns + "assemblyBinding");
            foreach (XElement assemblyBindingNode in assemblyBindingNodes) {
                IEnumerable<XElement> dependentAssemblyNodes = assemblyBindingNode.Descendants(ns + "dependentAssembly");
                foreach (XElement dependentAssemblyNode in dependentAssemblyNodes) {
                    IEnumerable<XElement> assemblyIdentityNode = dependentAssemblyNode.Descendants(ns + "assemblyIdentity");
                    IEnumerable<XElement> bindingRedirectNode = dependentAssemblyNode.Descendants(ns + "bindingRedirect");

                    string assemblyName = assemblyIdentityNode.Attributes("name").First().Value;
                    string oldVersion = bindingRedirectNode.Attributes("oldVersion").First().Value;
                    string newVersion = bindingRedirectNode.Attributes("newVersion").First().Value;
                    assembliesWithBindingRedirectInfo.Add(assemblyName, new BindingRedirectInfo { AssemblyName = assemblyName, OldVersion = oldVersion, NewVersion = newVersion });
                }
            }

            return assembliesWithBindingRedirectInfo;
        }

        public ComparisonResults CompareParsedConfigFiles(SortedDictionary<string, BindingRedirectInfo> bindingsWithExplicitRedirects, SortedDictionary<string, BindingRedirectInfo> bindingsWithoutExplicitRedirects) {
            var results = new ComparisonResults();
            var processedAssemblies = new HashSet<string>();

            foreach (var bindingWithExplicitRedirect in bindingsWithExplicitRedirects) {
                var matchingBindingWithoutExplicitRedirect = bindingsWithoutExplicitRedirects.FirstOrDefault(x => x.Key == bindingWithExplicitRedirect.Key).Value;

                if (matchingBindingWithoutExplicitRedirect == null) {
                    processedAssemblies.Add(bindingWithExplicitRedirect.Key);
                    results.BindingsOnlyWhenExplicitlySpecified.Add(bindingWithExplicitRedirect.Value);
                }
                else if (bindingWithExplicitRedirect.Value != matchingBindingWithoutExplicitRedirect) {
                    processedAssemblies.Add(bindingWithExplicitRedirect.Key);
                    results.BindingsWithDifferences.Add((bindingWithExplicitRedirect.Value, matchingBindingWithoutExplicitRedirect));
                }
            }

            foreach (var bindingWithoutExplicitRedirect in bindingsWithoutExplicitRedirects.Where(x => !processedAssemblies.Contains(x.Key))) {
                var matchingBindingWithExplicitRedirect = bindingsWithExplicitRedirects.FirstOrDefault(x => x.Key == bindingWithoutExplicitRedirect.Key).Value;

                if (matchingBindingWithExplicitRedirect == null) {
                    results.BindingsOnlyWhenNotExplicitlySpecified.Add(bindingWithoutExplicitRedirect.Value);
                }
                else if (matchingBindingWithExplicitRedirect != bindingWithoutExplicitRedirect.Value) {
                    results.BindingsWithDifferences.Add((matchingBindingWithExplicitRedirect, bindingWithoutExplicitRedirect.Value));
                }
            }

            return results;
        }
    }
}
