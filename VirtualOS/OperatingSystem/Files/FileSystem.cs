using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace VirtualOS.OperatingSystem.Files
{
    public class FileSystem
    {
        private readonly ZipArchive _fileSystem;
        private readonly Directory _rootDirectory;
        
        public List<string> Files
        {
            get
            {
                List<string> files = new List<string>();
                foreach (var entry in _fileSystem.Entries)
                {
                    files.Add(entry.FullName);
                }

                return files;
            }
        }

        public FileSystemUnit FindPath(string path)
        {
            return _rootDirectory.GetPath(path);
        }

        public FileSystem(string systemFile)
        {
            _fileSystem = ZipFile.Open(systemFile, ZipArchiveMode.Update);
            _rootDirectory = new Directory("/", "/");
            _rootDirectory.ParseFilesFromEntries(Files);
        }

        public ZipArchiveEntry GetFile(string path)
        {
            var file = _fileSystem.GetEntry(path);
            if (file == null)
            {
                throw  new NullReferenceException();
            }

            return file;
        }

        public List<string> ShowFiles(string location)
        {
            // Variable for info about files
            // Each line represents single file
            var info = new List<string>();
            
            var dir =( Directory) _rootDirectory.GetPath(location);
            if (dir != null)
            {
                if (dir.IsDirectory)
                {
                    info.Add($"{dir.Name}: {dir.ChildrenAmount} files.");
                    foreach (var child in dir.Children)
                    {
                        var fileType = child.IsDirectory ? "<d>" : "<f>";
                        info.Add($"{fileType} {child.Name} ({child.FullPath})");
                    }
                }
            }
            return info;
        }

        public void CreateFiles(List<string> names, bool isDirectory)
        {
            // If type of files is directory, then make slashes "/" is acceptable
            var acceptableSymbols = new Regex(@"^[a-zA-Z0-9]+$");
            if (isDirectory) acceptableSymbols = new Regex(@"^[a-zA-Z0-9/]+$");
            
            // Check for good name and if file already exists
            foreach (var name in names)
            {
                if (!acceptableSymbols.IsMatch(name))
                {
                    CommandLine.Error($"Invalid name for file: {name}");
                    return;
                }

                if (_fileSystem.GetEntry(name) != null)
                {
                    CommandLine.Error($"File {name} already exists");
                    return;
                }
            }
            // If all files have good names, create
            foreach (var name in names)
            {
                if (!name.EndsWith("/") && isDirectory)
                {
                    _fileSystem.CreateEntry(name + "/");
                    continue;
                }
                _fileSystem.CreateEntry(name);
            }

            CommandLine.ColorLog("Files created", ConsoleColor.DarkBlue);
        }

        public void RemoveFiles(List<string> files)
        {
            foreach (var file in files)
            {
                _fileSystem.GetEntry(file)?.Delete();
            }
            CommandLine.ColorLog("Files deleted.", ConsoleColor.DarkGreen);
        }

        // First parameter is a name of a file to ( create if not exists ) write to
        public void WriteFile(List<string> parameters)
        {
            var file = _fileSystem.GetEntry(parameters[0]) ?? _fileSystem.CreateEntry(parameters[0]);

            // Remove file name and leave only text
            parameters.RemoveAt(0);
            using (StreamWriter writer = new StreamWriter(file.Open()))
            {
                foreach (var word in parameters)
                {
                    writer.Write($"{word} ");
                }
            }
            CommandLine.ColorLog("Text is written to file.", ConsoleColor.Green);
        }
        
        public void ReadFile(string name)
        {
            var file = _fileSystem.GetEntry(name);
            if (file == null)
            {
                CommandLine.Error($"File {name} is not found");
                return;
            }

            using (StreamReader reader = new StreamReader(file.Open()))
            {
                CommandLine.DefaultLog(reader.ReadToEnd());
            }
        }
        public void Close()
        {
            _fileSystem.Dispose();
        }
    }
}