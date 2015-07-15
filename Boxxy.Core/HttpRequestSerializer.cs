using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Boxxy.Core
{
    internal class HttpRequestSerializer
    {
        public string ToString(HttpListenerRequest request) {
            NameValueCollection headers = request.Headers;
            foreach (var header in headers.AllKeys) {
                var values = headers.GetValues(header);
                Debug.Assert(values != null, "values != null");

                string valuesString = String.Join(" ", values);
                Console.WriteLine("{0}: {1}", header, valuesString);
            }

            string serializedHeaders = "";
            string serializedBody = new StreamReader(request.InputStream).ReadToEnd();

            return String.Format("{0}\n{1}", serializedHeaders, serializedBody);
        }


        public IncomingHttpRequest FromString(string input) {
            var request = new IncomingHttpRequest();


            return request;
        }
    }
}