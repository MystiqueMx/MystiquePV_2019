using MystiqueNative.Helpers;
using MystiqueNative.Helpers.Conekta;
using MystiqueNative.Interfaces;
using MystiqueNative.Models.Conekta;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.Services
{
    internal static class ConektaApi
    {
        internal static class Tokens
        {
            public const string EndPoint = "/tokens";
            public static async Task<JObject> Create(Card card)
            {
                try
                {

                    var fingerprint = ServiceLocator.Instance.Get<IConektaSessionId>().GetDeviceFingerPrint();
                    ServiceLocator.Instance.Get<IConektaSessionId>().SetFingerprint(fingerprint);
                    await Task.Delay(500); // Esperar a que cargue el webview en la plataforma xD
                    var content =
                        $@"{{""card"":{{""name"":""{card.name}"",""number"":""{card.number}"",""cvc"":""{card.cvc}"",""exp_month"":""{card.exp_month}"",""exp_year"":""{card.exp_year}"", ""device_fingerprint"":""{fingerprint}""}}}}";

                    var conn = new Connection(MystiqueApp.DevicePlatform);
                    var responseString = await conn.request(content, EndPoint);
                    var result = JObject.Parse(responseString);
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
        }
    }
}
