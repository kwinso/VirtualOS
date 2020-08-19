namespace VirtualOS.OperatingSystem
{
    // This class represents current user in the system
    public class SystemUser
    {
        public string Name { get; private set; } // username of a user in the system
        public string HomeDir { get; private set; } // location of the home directory

        public SystemUser (string name, string home)
        {
            Name = name;
            HomeDir = home;
        }
    }
}