using System.Collections.Generic;
using System.Xml.Serialization;

namespace BindingRedirectChecker.Models {
    [XmlRoot(ElementName = "assemblyIdentity")]
    public class AssemblyIdentity {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "publicKeyToken")]
        public string PublicKeyToken { get; set; }

        [XmlAttribute(AttributeName = "culture")]
        public string Culture { get; set; }
    }

    [XmlRoot(ElementName = "bindingRedirect")]
    public class BindingRedirect {
        [XmlAttribute(AttributeName = "oldVersion")]
        public string OldVersion { get; set; }

        [XmlAttribute(AttributeName = "newVersion")]
        public string NewVersion { get; set; }
    }

    [XmlRoot(ElementName = "dependentAssembly")]
    public class DependentAssembly {
        [XmlElement(ElementName = "assemblyIdentity")]
        public AssemblyIdentity AssemblyIdentity { get; set; }

        [XmlElement(ElementName = "bindingRedirect")]
        public BindingRedirect BindingRedirect { get; set; }
    }

    [XmlRoot(ElementName = "assemblyBinding")]
    public class AssemblyBinding {
        [XmlElement(ElementName = "dependentAssembly")]
        public DependentAssembly DependentAssembly { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "runtime")]
    public class Runtime {
        [XmlElement(ElementName = "assemblyBinding")]
        public List<AssemblyBinding> AssemblyBinding { get; set; }
    }

    [XmlRoot(ElementName = "configuration")]
    public class Configuration {
        [XmlElement(ElementName = "runtime")] public Runtime Runtime { get; set; }
    }
}