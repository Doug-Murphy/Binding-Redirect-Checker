namespace BindingRedirectChecker.Models {
    public record BindingRedirectInfo {
        public string OldVersion { get; init; }

        public string NewVersion { get; init; }
    }
}