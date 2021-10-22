using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NugetVersionExtractor
{
    public class MarkdownTable
    {
        private List<MarkdownRow> Rows { get; } = new();
        private int MaxPackageName => Rows.Max(r => r.PackageName.Length);
        private int MaxPackageVersion => Rows.Max(r => r.PackageVersion.Length);
        
        public static MarkdownTable Parse(FileInfo file)
        {
            MarkdownTable table = new();
            foreach (string line in FilterOutPackageReferenceLines(file))
            {
                if (IsValid(line))
                {
                    table.Add(line);
                }
            }

            return table;
        }

        public void Add(string line)
        {
            Match match = Program.PackageRegex.Match(line);
            Rows.Add(new (match.Groups[1].Value, match.Groups[2].Value));
        }
        public void Print()
        {
            PrintTableHeader();
            
            foreach (MarkdownRow row in Rows.Skip(1))
            {
                PrintRow(row);
            }
            
            Console.WriteLine();
        }
        
        #region Print helpers
        private void PrintTableHeader()
        {
            MarkdownRow headerRow = new("Name", "Version", true);
            Rows.Add(headerRow);
            
            PrintRow(headerRow);
        }
        private void PrintSeperator()
        {
            string nameFormat = $":-{new string('-', MaxPackageName - 2)}";
            string versionFormat = $":-{new string('-', MaxPackageVersion - 2)}";
            Console.WriteLine($"| {nameFormat} | {versionFormat} |");
        }
        private void PrintRow(MarkdownRow row)
        {
            if (row.IsHeader) PrintSeperator();
            
            string namePadding = new(' ', MaxPackageName - row.PackageName.Length);
            string versionPadding = new(' ', MaxPackageVersion - row.PackageVersion.Length);
            Console.WriteLine($"| {row.PackageName}{namePadding} | {row.PackageVersion}{versionPadding} |");
        }
        #endregion
        
        #region Parse Helpers
        private static bool IsValid(string line)
        {
            Match match = Program.PackageRegex.Match(line);
            if (!match.Success) return false;
            
            if (match.Groups.Count != 3)
            {
                Program.PrintWarning($"The following item has {match.Groups.Count} matches instead of 3: \"{line}\"");
                Console.ReadKey();
                return false;
            }

            int c = 0;
            foreach (Group group in match.Groups)
            {
                if (group.Captures.Count >= 0) continue;

                Program.PrintWarning($"Group {++c} does not have any captured value: \"{line}\"");
                Console.ReadKey();
                return false;
            }

            return true;
        }
        private static List<string> FilterOutPackageReferenceLines(FileInfo file)
        {
            return File.ReadAllLines(file.FullName)
                       .Where(l => l.Contains("PackageReference"))
                       .ToList();
        }
        #endregion
    }
}