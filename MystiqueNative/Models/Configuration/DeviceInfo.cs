using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Configuration
{
    public class DeviceInfo
    {
        public string Id { get; set; }
        public string Model { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
        public string ConnectionType { get; set; }
    }
}
