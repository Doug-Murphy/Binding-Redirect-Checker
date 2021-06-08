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

            //TODO: Don't use .Count to determine which to iterate through
            //This has a gap about if there happen to be the same #, yet the binding redirects are different
            //Ex: With explicit contains one less binding than without (shouldn't be possible, but let's say Newtonsoft) and without explicit contains one less binding than with (let's say RestSharp).
            //We won't be showing the proper comparison then.
            if (bindingsWithExplicitRedirects.Count > bindingsWithoutExplicitRedirects.Count) {
                foreach (var bindingWithExplicitRedirect in bindingsWithExplicitRedirects) {
                    var matchingBindingWithoutExplicitRedirect = bindingsWithoutExplicitRedirects.FirstOrDefault(x => x.Key == bindingWithExplicitRedirect.Key).Value;

                    if (matchingBindingWithoutExplicitRedirect == null) {
                        results.BindingsOnlyWhenExplicitlySpecified.Add(bindingWithExplicitRedirect.Value);
                    }
                    else if (bindingWithExplicitRedirect.Value != matchingBindingWithoutExplicitRedirect) {
                        results.BindingsWithDifferences.Add((bindingWithExplicitRedirect.Value, matchingBindingWithoutExplicitRedirect));
                    }
                }
            }
            else {
                foreach (var bindingWithoutExplicitRedirect in bindingsWithoutExplicitRedirects) {
                    var matchingBindingWithExplicitRedirect = bindingsWithExplicitRedirects.FirstOrDefault(x => x.Key == bindingWithoutExplicitRedirect.Key).Value;

                    if (matchingBindingWithExplicitRedirect == null) {
                        results.BindingsOnlyWhenNotExplicitlySpecified.Add(bindingWithoutExplicitRedirect.Value);
                    }
                    else if (matchingBindingWithExplicitRedirect != bindingWithoutExplicitRedirect.Value) {
                        results.BindingsWithDifferences.Add((matchingBindingWithExplicitRedirect, bindingWithoutExplicitRedirect.Value));
                    }
                }
            }

            return results;
        }
    }
}
