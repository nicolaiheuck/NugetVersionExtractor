namespace NugetVersionExtractor
{
    public class MarkdownRow
    {
        public string PackageName { get; }
        public string PackageVersion { get; }
        public bool IsHeader { get; }
        public MarkdownRow(string packageName, string packageVersion, bool isHeader = false)
        {
            PackageName = packageName;
            PackageVersion = packageVersion;
            IsHeader = isHeader;
        }
    }
}