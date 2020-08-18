namespace VirtualOS.OperatingSystem
{
    public class SystemUser
    {
        public string Name { get; private set; }
        public string HomeDir { get; private set; }

        public SystemUser (string name, string home)
        {
            Name = name;
            HomeDir = home;
        }
    }
}