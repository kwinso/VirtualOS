namespace VirtualOS.OperatingSystem.Files
{
    public class File : FileSystemUnit
    {
        // Just to inherit all File System Unit Fields.
        public File(string name, string fullPath) : base(name, fullPath, false)
        {
        }
    }
}