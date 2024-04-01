using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Interfaces
{
    public interface IConektaSessionId
    {
        string GetDeviceFingerPrint();
        void SetFingerprint(string fingerprint);
    }
}
