using BindingRedirectChecker.Helpers;
using BindingRedirectChecker.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BindingRedirectChecker.Tests.HelpersTests {
    public class ParsingHelperTests {
        private readonly ParsingHelper _parsingHelper;

        public ParsingHelperTests() {
            _parsingHelper = new ParsingHelper();
        }

        [Test(Description = "Each config file contains one assembly binding redirect that isn't in the other.")]
        public void Test_CompareParsedConfigFiles_Each_Config_With_A_Unique_Redirect() {
            var bindingsWithExplicitRedirects = new SortedDictionary<string, BindingRedirectInfo> {
                { "Newtonsoft.Json", new BindingRedirectInfo { AssemblyName = "Newtonsoft.Json", OldVersion = "0.0.0.0-12.0.0.0", NewVersion = "12.0.0.0" } },
                { "RestSharp", new BindingRedirectInfo { AssemblyName = "RestSharp", OldVersion = "0.0.0.0-106.0.1.0", NewVersion = "106.0.1.0" } },
            };

            var bindingsWithoutExplicitRedirects = new SortedDictionary<string, BindingRedirectInfo> {
                { "RestSharp", new BindingRedirectInfo { AssemblyName = "RestSharp", OldVersion = "0.0.0.0-106.0.1.0", NewVersion = "106.0.1.0" } },
                { "System.Reflection", new BindingRedirectInfo { AssemblyName = "System.Reflection", OldVersion = "0.0.0.0-4.1.1.0", NewVersion = "4.1.1.0" } }
            };

            ComparisonResults comparisonResults = _parsingHelper.CompareParsedConfigFiles(bindingsWithExplicitRedirects, bindingsWithoutExplicitRedirects);

            Assert.That(comparisonResults.BindingsOnlyWhenExplicitlySpecified, Has.Count.EqualTo(1));
            Assert.That(comparisonResults.BindingsOnlyWhenNotExplicitlySpecified, Has.Count.EqualTo(1));
            Assert.That(comparisonResults.BindingsWithDifferences, Is.Empty);

            Assert.That(comparisonResults.BindingsOnlyWhenExplicitlySpecified.First().AssemblyName, Is.EqualTo("System.Reflection"));
            Assert.That(comparisonResults.BindingsOnlyWhenExplicitlySpecified.First().OldVersion, Is.EqualTo("0.0.0.0-4.1.1.0"));
            Assert.That(comparisonResults.BindingsOnlyWhenExplicitlySpecified.First().NewVersion, Is.EqualTo("4.1.1.0"));

            Assert.That(comparisonResults.BindingsOnlyWhenNotExplicitlySpecified.First().AssemblyName, Is.EqualTo("Newtonsoft.Json"));
            Assert.That(comparisonResults.BindingsOnlyWhenNotExplicitlySpecified.First().OldVersion, Is.EqualTo("0.0.0.0-13.0.0.0"));
            Assert.That(comparisonResults.BindingsOnlyWhenNotExplicitlySpecified.First().NewVersion, Is.EqualTo("13.0.0.0"));
        }


        [Test(Description = "A binding redirect exists for an assembly only when not explicitly specified.")]
        public void Test_CompareParsedConfigFiles_Not_Explicit_Contains_Extra_Binding() {
            var bindingsWithExplicitRedirects = new SortedDictionary<string, BindingRedirectInfo> {
                { "RestSharp", new BindingRedirectInfo { AssemblyName = "RestSharp", OldVersion = "0.0.0.0-106.0.1.0", NewVersion = "106.0.1.0" } },
                { "System.Reflection", new BindingRedirectInfo { AssemblyName = "System.Reflection", OldVersion = "0.0.0.0-4.1.1.0", NewVersion = "4.1.1.0" } }
            };

            var bindingsWithoutExplicitRedirects = new SortedDictionary<string, BindingRedirectInfo> {
                { "Newtonsoft.Json", new BindingRedirectInfo { AssemblyName = "Newtonsoft.Json", OldVersion = "0.0.0.0-13.0.0.0", NewVersion = "13.0.0.0" } },
                { "RestSharp", new BindingRedirectInfo { AssemblyName = "RestSharp", OldVersion = "0.0.0.0-106.0.1.0", NewVersion = "106.0.1.0" } },
                { "System.Reflection", new BindingRedirectInfo { AssemblyName = "System.Reflection", OldVersion = "0.0.0.0-4.1.1.0", NewVersion = "4.1.1.0" } }
            };

            ComparisonResults comparisonResults = _parsingHelper.CompareParsedConfigFiles(bindingsWithExplicitRedirects, bindingsWithoutExplicitRedirects);

            Assert.That(comparisonResults.BindingsOnlyWhenExplicitlySpecified, Is.Empty);
            Assert.That(comparisonResults.BindingsOnlyWhenNotExplicitlySpecified, Has.Count.EqualTo(1));
            Assert.That(comparisonResults.BindingsWithDifferences, Is.Empty);

            Assert.That(comparisonResults.BindingsOnlyWhenNotExplicitlySpecified.First().AssemblyName, Is.EqualTo("Newtonsoft.Json"));
            Assert.That(comparisonResults.BindingsOnlyWhenNotExplicitlySpecified.First().OldVersion, Is.EqualTo("0.0.0.0-13.0.0.0"));
            Assert.That(comparisonResults.BindingsOnlyWhenNotExplicitlySpecified.First().NewVersion, Is.EqualTo("13.0.0.0"));
        }


        [Test(Description = "A binding redirect exists for an assembly only when explicitly specified.")]
        public void Test_CompareParsedConfigFiles_Explicit_Contains_Extra_Binding() {
            var bindingsWithExplicitRedirects = new SortedDictionary<string, BindingRedirectInfo> {
                { "Newtonsoft.Json", new BindingRedirectInfo { AssemblyName = "Newtonsoft.Json", OldVersion = "0.0.0.0-12.0.0.0", NewVersion = "12.0.0.0" } },
                { "RestSharp", new BindingRedirectInfo { AssemblyName = "RestSharp", OldVersion = "0.0.0.0-106.0.1.0", NewVersion = "106.0.1.0" } },
                { "System.Reflection", new BindingRedirectInfo { AssemblyName = "System.Reflection", OldVersion = "0.0.0.0-4.1.1.0", NewVersion = "4.1.1.0" } }
            };

            var bindingsWithoutExplicitRedirects = new SortedDictionary<string, BindingRedirectInfo> {
                { "RestSharp", new BindingRedirectInfo { AssemblyName = "RestSharp", OldVersion = "0.0.0.0-106.0.1.0", NewVersion = "106.0.1.0" } },
                { "System.Reflection", new BindingRedirectInfo { AssemblyName = "System.Reflection", OldVersion = "0.0.0.0-4.1.1.0", NewVersion = "4.1.1.0" } }
            };

            ComparisonResults comparisonResults = _parsingHelper.CompareParsedConfigFiles(bindingsWithExplicitRedirects, bindingsWithoutExplicitRedirects);

            Assert.That(comparisonResults.BindingsOnlyWhenExplicitlySpecified, Has.Count.EqualTo(1));
            Assert.That(comparisonResults.BindingsOnlyWhenNotExplicitlySpecified, Is.Empty);
            Assert.That(comparisonResults.BindingsWithDifferences, Is.Empty);

            Assert.That(comparisonResults.BindingsOnlyWhenExplicitlySpecified.First().AssemblyName, Is.EqualTo("Newtonsoft.Json"));
            Assert.That(comparisonResults.BindingsOnlyWhenExplicitlySpecified.First().OldVersion, Is.EqualTo("0.0.0.0-12.0.0.0"));
            Assert.That(comparisonResults.BindingsOnlyWhenExplicitlySpecified.First().NewVersion, Is.EqualTo("12.0.0.0"));
        }

        [Test(Description = "Both configs have the same bindings, but with different versions.")]
        public void Test_CompareParsedConfigFiles_SameBindings_Different_Versions() {
            var bindingsWithExplicitRedirects = new SortedDictionary<string, BindingRedirectInfo> {
                { "Newtonsoft.Json", new BindingRedirectInfo { AssemblyName = "Newtonsoft.Json", OldVersion = "0.0.0.0-12.0.0.0", NewVersion = "12.0.0.0" } },
                { "RestSharp", new BindingRedirectInfo { AssemblyName = "RestSharp", OldVersion = "0.0.0.0-106.0.1.0", NewVersion = "106.0.1.0" } }
            };

            var bindingsWithoutExplicitRedirects = new SortedDictionary<string, BindingRedirectInfo> {
                { "Newtonsoft.Json", new BindingRedirectInfo { AssemblyName = "Newtonsoft.Json", OldVersion = "0.0.0.0-13.0.0.0", NewVersion = "13.0.0.0" } },
                { "RestSharp", new BindingRedirectInfo { AssemblyName = "RestSharp", OldVersion = "0.0.0.0-106.0.1.0", NewVersion = "106.0.1.0" } }
            };


            ComparisonResults comparisonResults = _parsingHelper.CompareParsedConfigFiles(bindingsWithExplicitRedirects, bindingsWithoutExplicitRedirects);

            Assert.That(comparisonResults.BindingsOnlyWhenExplicitlySpecified, Is.Empty);
            Assert.That(comparisonResults.BindingsOnlyWhenNotExplicitlySpecified, Is.Empty);
            Assert.That(comparisonResults.BindingsWithDifferences, Has.Count.EqualTo(1));

            Assert.That(comparisonResults.BindingsWithDifferences.First().withExplicitRedirect.AssemblyName, Is.EqualTo("Newtonsoft.Json"));
            Assert.That(comparisonResults.BindingsWithDifferences.First().withExplicitRedirect.OldVersion, Is.EqualTo("0.0.0.0-12.0.0.0"));
            Assert.That(comparisonResults.BindingsWithDifferences.First().withExplicitRedirect.NewVersion, Is.EqualTo("12.0.0.0"));

            Assert.That(comparisonResults.BindingsWithDifferences.First().withoutExplicitRedirect.AssemblyName, Is.EqualTo("Newtonsoft.Json"));
            Assert.That(comparisonResults.BindingsWithDifferences.First().withoutExplicitRedirect.OldVersion, Is.EqualTo("0.0.0.0-13.0.0.0"));
            Assert.That(comparisonResults.BindingsWithDifferences.First().withoutExplicitRedirect.NewVersion, Is.EqualTo("13.0.0.0"));

            Assert.That(comparisonResults.BindingsWithDifferences, Does.Not.Contain((new BindingRedirectInfo { AssemblyName = "RestSharp", OldVersion = "0.0.0.0-106.0.1.0", NewVersion = "106.0.1.0" }, new BindingRedirectInfo { AssemblyName = "RestSharp", OldVersion = "0.0.0.0-106.0.1.0", NewVersion = "106.0.1.0" })));
        }
    }
}
