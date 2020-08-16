using System.Collections.Generic;

namespace VirtualOS
{
    public class SystemUser
    {
        public string Name { get; private set; }
        public string HomeDir;
        private List<string> _groups = new List<string>();

        public SystemUser (string name, string home)
        {
            Name = name;
            HomeDir = home;
        }

        public void AddInGroup(string group) => _groups.Add(group);
        public bool InGroup(string group) => _groups.Contains(group);
    }
}