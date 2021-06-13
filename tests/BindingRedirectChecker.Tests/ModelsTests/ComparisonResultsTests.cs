using BindingRedirectChecker.Models;
using NUnit.Framework;

namespace BindingRedirectChecker.Tests.ModelsTests {
    public class ComparisonResultsTests {
        [Test]
        public void Test_ComparisonResults_ToString() {
            var comparisonResults = new ComparisonResults();

            comparisonResults.BindingsOnlyWhenExplicitlySpecified.Add(new BindingRedirectInfo { AssemblyName = "Newtonsoft.Json", NewVersion = "13.0.0.0", OldVersion = "0.0.0.0-13.0.0.0" });
            comparisonResults.BindingsOnlyWhenNotExplicitlySpecified.Add(new BindingRedirectInfo { AssemblyName = "office", NewVersion = "14.0.0.0", OldVersion = "0.0.0.0-14.0.0.0" });
            comparisonResults.BindingsWithDifferences.Add((new BindingRedirectInfo { AssemblyName = "RestSharp", NewVersion = "105.0.0.0", OldVersion = "0.0.0.0-105.0.0.0" }, new BindingRedirectInfo { AssemblyName = "RestSharp", NewVersion = "106.0.0.0", OldVersion = "0.0.0.0-106.0.0.0" }));

            var comparisonResultsString = comparisonResults.ToString();

            Assert.That(comparisonResultsString, Is.EqualTo(@"Bindings present only when explicitly specified:
A binding redirect for Newtonsoft.Json directing versions 0.0.0.0-13.0.0.0 to 13.0.0.0 was only found when explicitly set. This means you shouldn't need it, but you should confirm that with runtime checking.
Bindings present only when not explicitly specified:
A binding redirect for office directing versions 0.0.0.0-14.0.0.0 to 14.0.0.0 was only found when not explicitly set. This should not happen. It should be fine, but you should confirm that with runtime checking.
Bindings that are different:
Binding redirect for RestSharp is different.
With explicit redirect: OldVersion=0.0.0.0-105.0.0.0, NewVersion=105.0.0.0
Without explicit redirect: OldVersion=0.0.0.0-106.0.0.0, NewVersion=106.0.0.0
"));
        }
    }
}
