using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.Helpers
{
    public static class RestApiClient
    {
        public static Task<string> Get(string url, Dictionary<string, string> urlParameters = null)
        {
            if (urlParameters != null)
                url = url + ToQueryString(urlParameters);

            RestRequest request = new RestRequest(method: Method.GET);
            RestClient client = new RestClient(url);
            client.AddDefaultHeader("X-Api-Secret", Configuration.MystiqueApiV2Config.MystiqueAppSecret);

            var taskWrapper = new TaskCompletionSource<string>();

#if DEBUG
            Console.WriteLine("~RestAPIClient > GET | REQUEST : url " + url);
#endif
            // BODY 
            // request.AddParameter("name", "value");
            // HTTP Headers
            // request.AddHeader("header", "value");

            // add files upload
            //request.AddFile(path);

            //~~~~~~~ SYNC
            // execute the request
            //IRestResponse response = client.Execute(request);
            //var content = response.Content;
            //OR
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            //RestResponse<Person> response2 = client.Execute<Person>(request);
            //var name = response2.Data.Name;

            //~~~~~~~ ASYNC
            client.ExecuteAsync(request, response => {
#if DEBUG
                Console.WriteLine("~RestAPIClient > GET | Response : " + response.Content);
#endif
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    taskWrapper.TrySetException(new HttpRequestException("Failed to get a response"));
                taskWrapper.TrySetResult(response.Content);
            });
            //OR
            // async with deserialization
            //var asyncHandle = client.ExecuteAsync<Person>(request, response => {
            //    taskWrapper.TrySetResult(response)
            //});
            // abort the request on demand
            //asyncHandle.Abort();

            return taskWrapper.Task;


        }
        public static Task<string> Post(string url, Dictionary<string, string> parameters = null)
        {
            RestRequest request = new RestRequest(method: Method.POST);
            RestClient client = new RestClient(url);
            client.AddDefaultHeader("X-Api-Secret", Configuration.MystiqueApiV2Config.MystiqueAppSecret);

            var taskWrapper = new TaskCompletionSource<string>();

#if DEBUG
            Console.WriteLine("~RestAPIClient > POST | REQUEST > url: " + url);
#endif
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
#if DEBUG
                    Console.WriteLine("~RestAPIClient > POST | PARAMS > " + p.Key + ":" + p.Value);
#endif
                    request.AddParameter(p.Key, p.Value);
                }
            }

            // HTTP Headers
            // request.AddHeader("header", "value");

            // add files upload
            //request.AddFile(path);

            //~~~~~~~ SYNC
            // execute the request
            //IRestResponse response = client.Execute(request);
            //var content = response.Content;
            //OR
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            //RestResponse<Person> response2 = client.Execute<Person>(request);
            //var name = response2.Data.Name;

            //~~~~~~~ ASYNC
            client.ExecuteAsync(request, response => {
#if DEBUG
                Console.WriteLine("~RestAPIClient > GET | Response : " + response.Content);
#endif
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    taskWrapper.TrySetException(new HttpRequestException("Failed to get a response"));

                taskWrapper.TrySetResult(response.Content);
            });
            //OR
            // async with deserialization
            //var asyncHandle = client.ExecuteAsync<Person>(request, response => {
            //    taskWrapper.TrySetResult(response)
            //});
            // abort the request on demand
            //asyncHandle.Abort();

            return taskWrapper.Task;


        }
        public static async Task<string> Fetch(string url, string method = "GET", string body = "", Dictionary<string, string> urlParameters = null)
        {
            HttpResponseMessage res;
            if (urlParameters != null)
                url = url + ToQueryString(urlParameters);

            Debug.WriteLine(String.Format("| RestAPIClient : Fetch, Enviando peticion | url> {0} body> {1} method> {2} ", url, body, method));
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-Api-Secret", Configuration.MystiqueApiV2Config.MystiqueAppSecret);
                    StringContent req;
                    switch (method)
                    {
                        case "GET":
                            res = await client.GetAsync(url);
                            break;
                        case "POST":
                            req = new StringContent(body, Encoding.UTF8, "text/json");
                            res = await client.PostAsync(url, req);
                            break;
                        case "PUT":
                            req = new StringContent(body, Encoding.UTF8, "text/json");
                            res = await client.PutAsync(url, req);
                            break;
                        case "DELETE":
                            res = await client.DeleteAsync(url);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    res.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("| RestAPIClient : Fetch, Error al obtener una respuesta del servidor | ex>", ex.Message);
                return string.Empty;
            }

            try
            {
                string content = await res.Content.ReadAsStringAsync();


                if (string.IsNullOrEmpty(content))
                    throw new FormatException("La respuesta esta vacia");


                return content;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("| RestAPIClient : Fetch, Error al leer la respuesta del servidor | ex>", ex.Message);
            }

            return string.Empty;
        }
        internal static Task<string> PutFile(byte[] file, string extension, string url, Dictionary<string, string> parameters = null)
        {
            RestRequest request = new RestRequest(method: Method.POST);
            RestClient client = new RestClient(url);
            client.AddDefaultHeader("X-Api-Secret", Configuration.MystiqueApiV2Config.MystiqueAppSecret);

            var taskWrapper = new TaskCompletionSource<string>();

#if DEBUG
            Console.WriteLine("~RestAPIClient > PutFile | REQUEST > url: " + url);
#endif
            foreach (var p in parameters)
            {
#if DEBUG
                Console.WriteLine("~RestAPIClient > PutFile | PARAMS > " + p.Key + ":" + p.Value);
#endif
                request.AddParameter(p.Key, p.Value);
            }

            request.AddFileBytes("file", file, "file" + extension, null);
            client.ExecuteAsync(request, response => {
#if DEBUG
                Console.WriteLine("~RestAPIClient > PutFile | Response : " + response.Content);
#endif
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    taskWrapper.TrySetException(new HttpRequestException("Failed to get a response"));

                taskWrapper.TrySetResult(response.Content);
            });

            return taskWrapper.Task;
        }
        private static string ToQueryString(Dictionary<string, string> dictionary)
        {
            var query = dictionary.Select(c => string.Format("{0}={1}", c.Key, c.Value)).ToArray();
            return "?" + string.Join("&", query);
        }

        public static Task<string> Post(string url, object body)
        {
            //RestRequest request = new RestRequest(method: Method.POST);
            var request = new RestSharp.Serializers.Newtonsoft.Json.RestRequest(Method.POST);
            var client = new RestClient(url);
            client.AddDefaultHeader("X-Api-Secret", Configuration.MystiqueApiV2Config.MystiqueAppSecret);

            //if (auth)
            //{
            //    if (RvsApp.LoggedUser == null) throw new UnauthorizedAccessException();
            //    request.AddHeader("Authorization", "Basic " + RvsApp.LoggedUser.BASIC_AUTH);
            //}

            var taskWrapper = new TaskCompletionSource<string>();

#if DEBUG
            Console.WriteLine("~RestAPIClient > Post | REQUEST > url: " + url + "\n");
            Console.WriteLine($"Body ~> {JsonConvert.SerializeObject(body)} \n");
#endif
            request.AddJsonBody(body);

            client.ExecuteAsync(request, response =>
            {
#if DEBUG
                Console.WriteLine("~RestAPIClient > Post | Response : " + response.Content + "\n");
#endif
                if (!response.IsSuccessful)
                {
                    taskWrapper.TrySetException(new TimeoutException("Failed to get a response"));
                }
                taskWrapper.TrySetResult(response.Content);
            });

            return taskWrapper.Task;


        }
    }
}
