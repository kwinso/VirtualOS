using System;
using System.Runtime.Serialization;

namespace VirtualOS
{
    [Serializable]
    public class SystemInfo : ISerializable
    {
        public string SystemName;
        public string SystemPath;
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", SystemName);
            info.AddValue("Path", SystemPath);
        }

        public SystemInfo(SerializationInfo info, StreamingContext context)
        {
            SystemName = (string) info.GetValue("Name", typeof(string));
            SystemPath = (string) info.GetValue("Path", typeof(string));
        }

        public SystemInfo(InstallingInfo info)
        {
            SystemName = info.SystemName;
            SystemPath = info.InstallingDir + "/" + SystemName;
        }
    }
}