using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Boxxy.Core
{
    public class Header
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Header(string name, string value) {
            Name = name;
            Value = value;
        }
    }

    public class IncomingHttpRequest
    {
        public IList<Header> Headers { get; set; }
        public HttpListenerResponse Response { get; set; }
        public Uri Uri { get; set; }
        public string HttpMethod { get; set; }
        public string Body { get; set; }

        // TODO - remove me later
        public HttpListenerRequest Request { get; set; }

        public IncomingHttpRequest() {
        }

        public IncomingHttpRequest(HttpListenerContext context) {
            Headers = new List<Header>();
            Request = context.Request;
            var request = context.Request;
            Response = context.Response;

            Body = new StreamReader(request.InputStream).ReadToEnd();
            Uri = request.Url;
            HttpMethod = request.HttpMethod;

            foreach (var headerKey in request.Headers.AllKeys) {
                string value = string.Join(",", request.Headers[headerKey]);
                Headers.Add(new Header(headerKey, value));
            }
        }

        public async Task RespondWith(HttpResponseMessage serverResponse) {
            var buffer = await serverResponse.Content.ReadAsByteArrayAsync();

            Response.ContentLength64 = buffer.Length;
            Response.ContentEncoding = Encoding.UTF8;

            var output = Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}