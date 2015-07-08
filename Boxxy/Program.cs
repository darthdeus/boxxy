using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Boxxy
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new HttpListener();

            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            var context = listener.GetContext();            

            var request = context.Request;
            Console.WriteLine(new HttpRequestSerializer().ToString(request));

            context.Response.Headers["Content-Type"] = "application/json";

            byte[] buffer = Encoding.UTF8.GetBytes("{\"x\": 1}");
            var output = context.Response.OutputStream;
            context.Response.ContentLength64 = buffer.Length;
            context.Response.ContentEncoding = Encoding.UTF8;

            output.Write(buffer, 0, buffer.Length);
            output.Close();
            listener.Stop();
        }
    }

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
