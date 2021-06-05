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
                    assembliesWithBindingRedirectInfo.Add(assemblyName, new BindingRedirectInfo { OldVersion = oldVersion, NewVersion = newVersion });
                }
            }

            return assembliesWithBindingRedirectInfo;
        }
    }
}
