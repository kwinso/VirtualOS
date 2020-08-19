using System;

namespace VirtualOS.OperatingSystem.Files
{
    public abstract class FileSystemUnit
    {
        public string Name { get; private set; } // Name of a unit
        public string FullPath { get; private set; } // Full path from parent to the current unit
        public bool IsDirectory { get; private set; } // Is unit a directory or a file

        protected FileSystemUnit(string name, string fullPath, bool isDir)
        {
            Name = name;
            FullPath = fullPath; 
            IsDirectory = isDir;
        }

    }
}