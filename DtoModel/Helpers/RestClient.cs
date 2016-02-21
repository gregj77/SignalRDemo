using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DtoModel.Helpers
{
    public class RestClient : IDisposable
    {
        private readonly HttpClient _client;

        public RestClient() : this(null)
        {
        }

        public RestClient(string userName)
        {
            HttpClientHandler handler = null;

            if (string.IsNullOrWhiteSpace(userName))
                handler = new HttpClientHandler
                {
                    Credentials = null,
                };
            else
                handler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential(userName, "dummy"),
                };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["ServiceBaseUrl"])
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> Get<T>(string url)
        {
            using (var response = await _client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsAsync<T>();
                return result;
            }
        }

        public async Task<TResponse> Put<TBody, TResponse>(string url, TBody body)
        {
            var response = await _client.PutAsJsonAsync(url, body);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<TResponse>();
        }

        public async Task Delete<TBody>(string url, TBody body)
        {
            var response = await _client.DeleteAsync(url + body);
            response.EnsureSuccessStatusCode();
        }

        public void Dispose()
        {
            _client.Dispose();            
        }
    }
}