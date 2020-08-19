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

        #region Paths Converting
        public static string ToZipFormat(string path)
        {
            //  Remove slash at the begging if needed
            if (path.StartsWith("/")) path = path.Substring(1);
            // IF path does not represent a file and does not ends with slash (directories in zip have to end with slash)
            if (!path.Contains(".") && !path.EndsWith("/")) path += "/";
            return path;
        }
        public static string ToAbsolutePath(string path, string currentLocation)
        {
            
            if (path.StartsWith(".."))
            {
                string pathToGo = "";
                
                if (path.StartsWith("../"))
                    pathToGo = path.Substring(3);
                
                return OneLevelUp(currentLocation) + pathToGo;
            }
            // Relative paths to current position
            if (path.StartsWith("."))
            {
                return NavigateRelative(path, currentLocation);
            }

            // Add slash if that's a folder and have no slash at end
            if (!path.EndsWith("/") && !path.Contains(".")) path += "/";
            // Absolute path
            if (path.StartsWith("/")) return path;
            else return currentLocation + path;
        }
        
        private static string OneLevelUp(string location)
        {
            return GoToParent(location);
        }
        // Navigating relative to the current directory
        private static string NavigateRelative(string path, string location)
        {
            // Stay were we were
            if (path == ".") return location;
            
            var locationToGo = path.Substring(2);
            locationToGo = location + locationToGo;
            return locationToGo;
        }
        
        // /path/to/file/ => /path/to/
        private static string GoToParent(string path)
        {
            path = path.Substring(0, path.LastIndexOf("/"));
            return path.Substring(0, path.LastIndexOf("/") + 1);;
        }
        

        #endregion
        
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
                        info.Add($"{fileType} {child.Name}");
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
        public void WriteFile(string text, string filepath, bool appending)
        {
            var file = _fileSystem.GetEntry(filepath) ?? _fileSystem.CreateEntry(filepath);

            if (appending) // Append text with new line if there's already some text
            {
                var previousText = ReadFile(filepath);
                if (!String.IsNullOrEmpty(previousText))
                    text = ReadFile(filepath) + "\n" + text;
            }
            else // Recreate file
            {
                file.Delete();
                file = _fileSystem.CreateEntry(filepath);
            }
            using (StreamWriter writer = new StreamWriter(file.Open()))
            {
                writer.Write(text);
            }
            CommandLine.ColorLog("Text is written to file.", ConsoleColor.Green);
        }
        
        public string ReadFile(string name)
        {
            var file = _fileSystem.GetEntry(name);
            if (file == null)
                return $"File {name} is not found";

            using (StreamReader reader = new StreamReader(file.Open()))
            {
                return reader.ReadToEnd();
            }
        }
        
        // TODO: Run this function on unpredicted system shutdown.
        public void Close()
        {
            _fileSystem.Dispose();
        }
    }
}