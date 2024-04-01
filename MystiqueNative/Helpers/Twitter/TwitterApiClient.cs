using MystiqueNative.Configuration;
using MystiqueNative.Models.Twitter;
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
    public static class TwitterApiClient
    {
        public static Task<string> Get(string url, RequestToken token, Dictionary<string, string> parameters = null)
        {
            string AuthHeader = Twitter.Authorization.GetAuthenticatedHeader(new Uri(url), token, HttpMethod.Get, parameters);
            if (parameters != null)
                url = url + ToQueryString(parameters);
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(method: Method.GET);

            var taskWrapper = new TaskCompletionSource<string>();

#if DEBUG
            Console.WriteLine("~TwitterApiClient > GET | REQUEST : url " + url);
#endif
            request.AddHeader("Authorization", AuthHeader);
            client.ExecuteAsync(request, response => {
#if DEBUG
                Console.WriteLine("~TwitterApiClient > GET | Response : " + response.Content);
#endif
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    taskWrapper.TrySetException(new HttpRequestException("Failed to get a response"));
                taskWrapper.TrySetResult(response.Content);
            });

            return taskWrapper.Task;


        }
        public static Task<string> Post(string url, RequestToken token, Dictionary<string, string> parameters = null)
        {
            RestRequest request = new RestRequest(method: Method.POST);
            RestClient client = new RestClient(url);
            string AuthHeader = Twitter.Authorization.GetAuthenticatedHeader(new Uri(url), token, HttpMethod.Post, parameters);

            var taskWrapper = new TaskCompletionSource<string>();

#if DEBUG
            Console.WriteLine("~TwitterApiClient > POST | REQUEST > url: " + url);
            Console.WriteLine("~TwitterApiClient > POST | REQUEST > Authorization: " + AuthHeader);
#endif
            foreach (var p in parameters)
            {
#if DEBUG
                Console.WriteLine("~TwitterApiClient > POST | PARAMS > " + p.Key + ":" + p.Value);
#endif
                request.AddParameter(p.Key, p.Value);
            }
            request.AddHeader("Authorization", AuthHeader);
            client.ExecuteAsync(request, response => {
#if DEBUG
                Console.WriteLine("~TwitterApiClient > POST | Response : " + response.Content);
#endif
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    taskWrapper.TrySetException(new HttpRequestException("Failed to get a response"));

                taskWrapper.TrySetResult(response.Content);
            });

            return taskWrapper.Task;


        }
        public static Task<string> RequestToken(string CallbackUrl)
        {
            var Url = TwitterApiConfig.RequestTokenPath;
            RestRequest request = new RestRequest(method: Method.POST);
            RestClient client = new RestClient(Url);
            string AuthHeader = Twitter.Authorization.GetRequestTokenHeader(new Uri(Url), CallbackUrl, HttpMethod.Post);

            var taskWrapper = new TaskCompletionSource<string>();

#if DEBUG
            Console.WriteLine("~TwitterApiClient > RequestToken | REQUEST > url: " + Url);
            Console.WriteLine("~TwitterApiClient > RequestToken | REQUEST > Authorization: " + AuthHeader);
#endif

            request.AddHeader("Authorization", AuthHeader);

            client.ExecuteAsync(request, response => {
#if DEBUG
                Console.WriteLine("~TwitterApiClient > RequestToken | Response : " + response.Content);
#endif
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    taskWrapper.TrySetException(new HttpRequestException("Failed to get a response"));

                taskWrapper.TrySetResult(response.Content);
            });

            return taskWrapper.Task;


        }
        public static Task<string> RequestAccessToken(RequestToken token)
        {
            var Url = TwitterApiConfig.AccessTokenPath;
            RestRequest request = new RestRequest(method: Method.POST);
            RestClient client = new RestClient(Url);
            string AuthHeader = Twitter.Authorization.GetAccessTokenHeader(new Uri(Url), token, HttpMethod.Post);

            var taskWrapper = new TaskCompletionSource<string>();

#if DEBUG
            Console.WriteLine("~TwitterApiClient > RequestAccessToken | REQUEST > url: " + Url);
            Console.WriteLine("~TwitterApiClient > RequestAccessToken | REQUEST > Authorization: " + AuthHeader);
#endif

            request.AddHeader("Authorization", AuthHeader);

            client.ExecuteAsync(request, response => {
#if DEBUG
                Console.WriteLine("~TwitterApiClient > RequestAccessToken | Response : " + response.Content);
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
    }
}
