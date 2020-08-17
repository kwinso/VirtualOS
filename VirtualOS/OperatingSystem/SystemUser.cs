using System.Collections.Generic;

namespace VirtualOS
{
    public class SystemUser
    {
        public string Name { get; private set; }

        public SystemUser (string name)
        {
            Name = name;
        }
    }
}