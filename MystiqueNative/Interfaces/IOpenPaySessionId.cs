using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.Interfaces
{
    public interface IOpenPaySessionId
    {
        Task<string> CreateDeviceSessionIdInternal(string merchantId, string apiKey, string baseUrl);
    }
}
