using MystiqueNative.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.Helpers.Conekta
{
    public class Connection
    {

        public string Platform { get; set; }

        public Connection(string platform)
        {
            Platform = platform;
        }

        public async Task<string> request(string Content, string EndPoint)
        {
            if (string.IsNullOrEmpty(EndPoint))
                throw new ArgumentException("endPoint empty");

            var client = new HttpClient();

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, ConektaApiConfig.UrlApi + EndPoint);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ConektaApiConfig.PublicKey}:")));
            requestMessage.Headers.Add("Accept", "application/vnd.conekta-v" + ConektaApiConfig.ApiVersion + "+json");

            switch (Platform)
            {
                case "Android":
                    requestMessage.Headers.Add("Conekta-Client-User-Agent", @"{""agent"": ""Conekta Android SDK""}");
                    break;
                case "iOS":
                    requestMessage.Headers.Add("Conekta-Client-User-Agent", @"{""agent"": ""Conekta iOS SDK""}");
                    break;
            }

            requestMessage.Content = new StringContent(Content, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(requestMessage);

            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}
