using System.Collections.Generic;
using System.Text;

namespace BindingRedirectChecker.Models {
    public class ComparisonResults {
        public List<BindingRedirectInfo> BindingsOnlyWhenExplicitlySpecified { get; set; } = new List<BindingRedirectInfo>();

        public List<BindingRedirectInfo> BindingsOnlyWhenNotExplicitlySpecified { get; set; } = new List<BindingRedirectInfo>();

        public List<(BindingRedirectInfo withExplicitRedirect, BindingRedirectInfo withoutExplicitRedirect)> BindingsWithDifferences { get; set; } = new List<(BindingRedirectInfo, BindingRedirectInfo)>();

        public override string ToString() {
            var sbOutput = new StringBuilder();

            if (BindingsOnlyWhenExplicitlySpecified.Count > 0) {
                sbOutput.AppendLine("Bindings present only when explicitly specified:");
            }
            foreach (BindingRedirectInfo bindingOnlyWhenExplicitlySpecified in BindingsOnlyWhenExplicitlySpecified) {
                sbOutput.AppendLine($"A binding redirect for {bindingOnlyWhenExplicitlySpecified.AssemblyName} directing versions {bindingOnlyWhenExplicitlySpecified.OldVersion} to {bindingOnlyWhenExplicitlySpecified.NewVersion} was only found when explicitly set. This means you shouldn't need it, but you should confirm that with runtime checking.");
            }

            if (BindingsOnlyWhenNotExplicitlySpecified.Count > 0) {
                sbOutput.AppendLine("Bindings present only when not explicitly specified:");
            }
            foreach (BindingRedirectInfo bindingOnlyWhenNotExplicitlySpecified in BindingsOnlyWhenNotExplicitlySpecified) {
                sbOutput.AppendLine($"A binding redirect for {bindingOnlyWhenNotExplicitlySpecified.AssemblyName} directing versions {bindingOnlyWhenNotExplicitlySpecified.OldVersion} to {bindingOnlyWhenNotExplicitlySpecified.NewVersion} was only found when not explicitly set. This should not happen. It should be fine, but you should confirm that with runtime checking.");
            }

            if (BindingsWithDifferences.Count > 0) {
                sbOutput.AppendLine("Bindings that are different:");
            }
            foreach ((BindingRedirectInfo withExplicitRedirect, BindingRedirectInfo withoutExplicitRedirect) bindingWithDifferences in BindingsWithDifferences) {
                sbOutput.AppendLine($"Binding redirect for {bindingWithDifferences.withExplicitRedirect.AssemblyName} is different.");
                sbOutput.AppendLine($"With explicit redirect: OldVersion={bindingWithDifferences.withExplicitRedirect.OldVersion}, NewVersion={bindingWithDifferences.withExplicitRedirect.NewVersion}");
                sbOutput.AppendLine($"Without explicit redirect: OldVersion={bindingWithDifferences.withoutExplicitRedirect.OldVersion}, NewVersion={bindingWithDifferences.withoutExplicitRedirect.NewVersion}");
            }

            return sbOutput.ToString();
        }
    }
}
