﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NugetVersionExtractor
{
    internal static class Program
    {
        public readonly static Regex PackageRegex = new("^[ ]*<PackageReference Include=\"([^\"]+)\" Version=\"([^\"]+)\".*>$", 
                                                        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static void Main(string[] args)
        {
            string folderPath = AskUserForPathToProjectFolder();
            Console.Clear();
            foreach (FileInfo file in GetProjectFiles(folderPath))
            {
                PrintSectionHeader(file);

                MarkdownTable table = MarkdownTable.Parse(file);
                
                table.Print();
            }

            Halt();
        }

        #region Helpers
        private static List<FileInfo> GetProjectFiles(string folderPath)
        {
            List<FileInfo> files = new DirectoryInfo(folderPath).GetFiles("*.csproj", SearchOption.AllDirectories)
                                                                .ToList();

            if (files.Count > 0) return files;

            PrintError("No .csproj files found");
            Console.ReadKey();
            Environment.Exit(1);
            return default;
        }
        
        private static void PrintSectionHeader(FileInfo file)
        {
            string fileName = file.Name.Replace(".csproj", "");
            Console.WriteLine($"## {fileName}");
        }
        public static void PrintWarning(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        private static void PrintError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        
        private static string AskUserForPathToProjectFolder()
        {
            Console.Clear();
            while (true)
            {
                Console.Write("Enter full path to project folder: ");
                string folderPath = Console.ReadLine();
                
                Console.Clear();
                if (Directory.Exists(folderPath)) return folderPath;
                
                PrintError("Invalid path, please try again");
            }
        }
        private static void Halt()
        {
            Console.WriteLine("\n\n\nPress X to exit");
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.X) return;
            }
        }
        #endregion
    }
}