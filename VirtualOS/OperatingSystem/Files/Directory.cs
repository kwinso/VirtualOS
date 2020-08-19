using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualOS.OperatingSystem.Files
{
    public class Directory : FileSystemUnit
    {
        public List<FileSystemUnit> Children = new List<FileSystemUnit>(); // Directory children

        public int ChildrenAmount => Children.Count;

        public Directory(string name, string fullPath) : base(name, fullPath, true)
        {
        }

        public FileSystemUnit GetPath(string path)
        {
            if (FullPath == path) return this;
            
            foreach (var child in Children)
            {
                if (child.FullPath == path) return child;
                
                if (child.IsDirectory) // Check for path in child's children
                {
                    var dir = child as Directory;
                    var found = dir.GetPath(path);
                    if (found != null) return found;
                }
            }

            return null;
        }

        private Directory FindSubDirectory(string name)
        {
            foreach (var dir in Children.Where(t => t is Directory ))
            {
                if (dir.Name == name) return (Directory) dir;
            }
            return null;
        }

        public void UpdatePaths(List<String> filePaths)
        {
            // Updating dir children.
            Children = new List<FileSystemUnit>();
            ParseFilesFromEntries(filePaths);
        }

        public void ParseFilesFromEntries(List<string> filePaths)
        {
            for (var i = 0; i < filePaths.Count; i++)
            {
                ParseChild(filePaths[i]);
            }
        }

        /*
            This method receives a zip path to entry, separates path to the array by slashes.
            IF path ends with "/", that means that's a directory
                After separating one level directory, e.g. "path/", we'll get array ["path", ""]
                Based on that data, we can say, that every path, that has length 2
                is a one-level directory(does not have children), so, we can just add it to the parent child dir
                
                Every other files needed to be re-parsed on the next level of directories
            IF path contains ".", like every file, we need to do same operations that we do with directories,
            but we need to create a file
        */
        private void ParseChild(string path)
        {
            // Separating path by slashes to get only names
            var filePath = path.Split("/");
            
            // Parsing Directory with slash on the end
            if (path.EndsWith('/'))
            {
                if (filePath.Length == 2)
                {
                    var fileName = filePath[0] + "/";
                    Children.Add(new Directory(fileName, FullPath + fileName));
                }
                else
                    ParseToNextLevel(path); // Reparse it 'till it will not have parents
            }
            else if (path.Contains("."))
            {
                if (filePath.Length == 1)
                {
                    var fileName = filePath[0];
                    Children.Add(new File(fileName, FullPath + fileName));
                }
                else
                    ParseToNextLevel(path);
            }
        }
        /*
            Find a parent directory at the begging of a path and starts a  process of parsing in this directory
        */
        private void ParseToNextLevel(string path)
        {
            var parentDirName = path.Split("/")[0] + "/"; // Finding very first parent in path
            
            var parentDir = FindSubDirectory(parentDirName);
            // Excluding parent directory name from path to parse only next level
            parentDir?.ParseChild(path.Replace(parentDir.Name, ""));
        }
    }
}