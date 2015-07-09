using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Boxxy.Core
{
    class HttpRequestSerializer
    {
        public string ToString(HttpListenerRequest request)
        {
            NameValueCollection headers = request.Headers;
            foreach (var header in headers.AllKeys)
            {
                var values = headers.GetValues(header);
                Debug.Assert(values != null, "values != null");

                string valuesString = string.Join(" ", values);
                Console.WriteLine("{0}: {1}", header, valuesString);
            }

            string serializedHeaders = "";
            string serializedBody = new StreamReader(request.InputStream).ReadToEnd();

            return string.Format("{0}\n{1}", serializedHeaders, serializedBody);
        }
    }
}