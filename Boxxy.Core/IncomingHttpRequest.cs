using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Boxxy.Core
{
    public class Header
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Header() {}

        public Header(string name, string value) {
            Name = name;
            Value = value;            
        }
    }

    public class IncomingHttpRequest
    {
        public IList<Header> Headers { get; set; }
        public Uri Uri { get; set; }
        public string HttpMethod { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsDeserialized { get; set; }
        public bool IsSent { get; set; }

        /// _originalClientResponseSocket attributes, initialized only when the request <code>IsSent</code> is true.
        public IList<Header> ResponseHeaders { get; set; }

        public string ResponseBody { get; set; }

        public string Destination { get; set; }

        /// <summary>
        /// Represents a connection which is used to forward the response from the destination server.
        /// </summary>
        [JsonIgnore] private readonly HttpListenerResponse _originalClientResponseSocket;

        public IncomingHttpRequest() {
            IsDeserialized = false;
            IsSent = false;
        }
        
        public IncomingHttpRequest(HttpListenerContext context, string destination) {
            Destination = destination;
            Headers = new List<Header>();
            _originalClientResponseSocket = context.Response;
            CreatedAt = DateTime.UtcNow;
            IsSent = false;

            var request = context.Request;
            Body = new StreamReader(request.InputStream).ReadToEnd();
            Uri = request.Url;
            HttpMethod = request.HttpMethod;

            ResponseHeaders = new List<Header>();

            foreach (var headerKey in request.Headers.AllKeys) {
                string value = string.Join(",", request.Headers[headerKey]);
                Headers.Add(new Header(headerKey, value));
            }
        }

        public static IncomingHttpRequest FromString(string str) {
            ParseResult<JsonObject> res = Json.Parse(str);

            if (res.Success)
            {
                var obj = res.Result.Item1;
                var dict = obj.Value;

                var request = new IncomingHttpRequest
                {
                    Uri = new Uri(dict["Uri"].ToRep<string>()),
                    Destination = dict["Destination"].ToRep<string>(),
                    HttpMethod = dict["HttpMethod"].ToRep<string>(),
                    CreatedAt = DateTime.Parse(dict["CreatedAt"].ToRep<string>()),
                    Body = dict["Body"].ToRep<string>(),
                    IsDeserialized = true
                };

                // TODO - headers

                return request;
            }
            else
            {
                throw new InvalidStorageFormat();
            }

            //var request = JsonConvert.DeserializeObject<IncomingHttpRequest>(str);
            //request.IsDeserialized = true;

            //return request;
        }

        public async Task Play() {
            var serverResponse = await Forward();

            if (!IsSent && !IsDeserialized && _originalClientResponseSocket != null) {
                IsSent = true;
                await RespondWith(serverResponse);
            } else {                
                Debug.WriteLine("The request was deserialized from disk, and therefore can't be forwarded back to the client.");
            }
        }

        private async Task<HttpResponseMessage> Forward() {
            HttpResponseMessage response;
            using (var client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)}) {
                var requestMethod = ParseHttpMethod(HttpMethod);
                var message = new HttpRequestMessage(requestMethod, Destination);

                foreach (var header in Headers) {
                    message.Headers.Add(header.Name, header.Value);
                }

                message.Headers.Host = new Uri(Destination).Host;

                if (requestMethod != System.Net.Http.HttpMethod.Get) {
                    message.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(Body));
                }

                response = await client.SendAsync(message);
            }

            ResponseHeaders.Clear();
            foreach (var header in response.Headers) {
                string value = string.Join(",", header.Value);
                ResponseHeaders.Add(new Header(header.Key, value));
            }
            
            ResponseBody = await response.Content.ReadAsStringAsync();
            return response;
        }

        private async Task RespondWith(HttpResponseMessage serverResponse) {
            var buffer = await serverResponse.Content.ReadAsByteArrayAsync();

            _originalClientResponseSocket.ContentLength64 = buffer.Length;
            _originalClientResponseSocket.ContentEncoding = Encoding.UTF8;

            var output = _originalClientResponseSocket.OutputStream;

            foreach (var header in ResponseHeaders) {
                // This is manually skipped since it results in an exception if added as a header.
                if (WebHeaderCollection.IsRestricted(header.Name) || header.Name == "Keep-Alive") {
                    continue;
                }
                _originalClientResponseSocket.Headers.Add(header.Name, header.Value);
            }           

            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private HttpMethod ParseHttpMethod(string httpMethod) {
            switch (httpMethod.ToUpper()) {
                case "GET":
                    return System.Net.Http.HttpMethod.Get;
                case "POST":
                    return System.Net.Http.HttpMethod.Post;
                case "PUT":
                    return System.Net.Http.HttpMethod.Put;
                case "DELETE":
                    return System.Net.Http.HttpMethod.Delete;
                case "OPTIONS":
                    return System.Net.Http.HttpMethod.Options;
                case "TRACE":
                    return System.Net.Http.HttpMethod.Trace;
                default:
                    var msg = string.Format("{0} is not a valid HTTP method", httpMethod);
                    throw new ArgumentException("httpMethod", msg);
            }
        }
    }
}