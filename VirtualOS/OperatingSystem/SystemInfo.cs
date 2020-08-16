using System;
using System.Runtime.Serialization;

namespace VirtualOS.OperatingSystem
{
    [Serializable]
    public class SystemInfo : ISerializable
    {
        public string SystemName { get; set; }

        public SystemInfo(string name)
        {
            SystemName = name;
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SystemName", SystemName);
        }
        public SystemInfo(SerializationInfo info, StreamingContext context)
        {
            SystemName = (string) info.GetValue("SystemName", typeof(string));
        }
        // A parameterless constructor for serialization
        public SystemInfo() {}
    }
}