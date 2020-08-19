using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace VirtualOS.OperatingSystem.Files
{
    public class FileSystem
    {
        private readonly ZipArchive _systemFile; // File where system is stored
        private readonly Directory _rootDirectory;

        private List<string> Files
        {
            get
            {
                List<string> files = new List<string>();
                foreach (var entry in _systemFile.Entries)
                {
                    files.Add(entry.FullName);
                }

                return files;
            }
        }

        public FileSystemUnit FindPath(string path) // Find path in the system.
        {
            return _rootDirectory.GetPath(path);
        }

        public FileSystem(string systemFile)
        {
            _systemFile = ZipFile.Open(systemFile, ZipArchiveMode.Update); // Open file by the path
            _rootDirectory = new Directory("", "/"); // Empty name, because it's a root of a system
            _rootDirectory.ParseFilesFromEntries(Files); // To Create file tree from zip entries
        }

        public ZipArchiveEntry GetFile(string path)
        {
            return _systemFile.GetEntry(path);
        }

        public List<string> ShowFiles(string location)
        {
            // Variable for info about files
            // Each line represents single child
            var info = new List<string>();
            
            var dir =( Directory) _rootDirectory.GetPath(location);
            if (dir != null)
            {
                // Format of showing: <d> (for directory) / <f> (for file) <filename>
                info.Add($"{dir.Name}: {dir.ChildrenAmount} files.");
                foreach (var child in dir.Children)
                {
                    var fileType = child.IsDirectory ? "<d>" : "<f>";
                    info.Add($"{fileType} {child.Name}");
                }
            }
            else
            {
                info.Add($"{location} does not exists.");
            }
            return info;
        }

        public void CreateSystemUnit(string filepath)
        {
            // Check for good name and if file already exists

            var existingFile = _rootDirectory.GetPath(filepath);
            if (existingFile != null)
            {
                CommandLine.Error($"File {filepath} already exists. Not created.");
                return;
            }

            var zipFormatPath = Path.ToZipFormat(filepath);
            
            // Add created file to the system
            _systemFile.CreateEntry(zipFormatPath);
            _rootDirectory.UpdatePaths(Files); // Update info about directories
        }

        public void RemoveFile(string path, bool isDirectory)
        {
            if (isDirectory)
            {
                path = Path.ToZipFormat(path); // Convert path to directory into zip format
                
                foreach (var filepath in Files)
                {
                    // Delete all files that was in deleted directory
                    if (filepath.StartsWith(path)) 
                    {
                        var fileToDelete = _systemFile.GetEntry(filepath);
                        fileToDelete?.Delete();
                    }
                }
            }
            else
            {
                var file = _systemFile.GetEntry(Path.ToZipFormat(path));
                if (file == null)
                {
                    CommandLine.Error($"Cannot remove {path}. File or directory does not exists. Skipped.");
                    return;
                }
                file.Delete();
            }

            _rootDirectory.UpdatePaths(Files); // Update info about directories
        }
        
        public void WriteToFile(string text, string filepath, bool appending)
        {
            // Create file if not exists
            var file = _systemFile.GetEntry(filepath) ?? _systemFile.CreateEntry(filepath);

            if (appending) // Append text with new line if there's already some text
            {
                var previousText = ReadFile(filepath);
                if (!String.IsNullOrEmpty(previousText))
                    text = ReadFile(filepath) + "\n" + text;
            }
            else // Recreate file
            {
                file.Delete();
                file = _systemFile.CreateEntry(filepath);
            }
            using (var writer = new StreamWriter(file.Open()))
            {
                writer.Write(text);
            }

            _rootDirectory.UpdatePaths(Files); // Update info about directories
        }
        
        public string ReadFile(string name)
        {
            var file = _systemFile.GetEntry(name);
            if (file == null)
                return $"File {name} is not found";

            using var reader = new StreamReader(file.Open());
            return reader.ReadToEnd();
        }

        public void Close()
        {
            _systemFile.Dispose(); // Save zip archive state.
        }
    }
}