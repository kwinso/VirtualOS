namespace VirtualOS.OperatingSystem.Files
{
    public abstract class FileSystemUnit
    {
        public string Name { get; private set; }
        public string FullPath { get; private set; }
        public bool IsDirectory { get; private set; }

        public FileSystemUnit(string name, string fullPath, bool isDir)
        {
            Name = name;
            FullPath = fullPath;
            IsDirectory = isDir;
        }

    }
}