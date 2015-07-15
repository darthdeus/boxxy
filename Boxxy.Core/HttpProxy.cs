using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Boxxy.Core
{
    public class HttpProxy
    {
        public bool IsRunning { get; private set; }

        private readonly string _destination;
        private readonly HttpListener _listener;
        private CancellationTokenSource _currentTcs;
        public IList<IncomingHttpRequest> IncomingRequests { get; private set; }

        public HttpProxy(string prefixUrl, string destination) {
            IncomingRequests = new List<IncomingHttpRequest>();
            _destination = destination;
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefixUrl);
            IsRunning = false;
        }

        public void Start() {
            _listener.Start();
            IsRunning = true;

            _currentTcs = new CancellationTokenSource();

            // Not really sure why this type isn't inferred properly.
            Task.Run((Action) ListenerLoop, _currentTcs.Token);
        }

        private void ListenerLoop() {
            while (true) {
                // TODO - handle manual stop from UI, which raises an exception if waiting on GetContext()
                var context = _listener.GetContext();
                HandleRequest(context).Wait();
            }
        }

        private async Task HandleRequest(HttpListenerContext context) {
            var incomingRequest = new IncomingHttpRequest(context);
            IncomingRequests.Add(incomingRequest);
            OnRequest(incomingRequest);

            var serverResponse = await Forward(incomingRequest);

            //incomingRequest.Request = null;
            //incomingRequest.Response = null;
            //var str = JsonConvert.SerializeObject(incomingRequest);
            //Console.WriteLine(str);

            //using (var writer = new StreamWriter("neco.json")) {
            //    writer.Write(str);
            //}

            //var hh = JsonConvert.DeserializeObject<IncomingHttpRequest>(str);
            //Console.WriteLine(hh);

            await Respond(incomingRequest, serverResponse);
        }

        private async Task<HttpResponseMessage> Forward(IncomingHttpRequest incomingRequest) {
            HttpResponseMessage response;
            using (var client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)}) {
                var request = incomingRequest.Request;

                var requestMethod = ParseHttpMethod(request.HttpMethod);
                var message = new HttpRequestMessage(requestMethod, _destination);

                // TODO - fix me
                //Console.WriteLine(new HttpRequestSerializer().ToString(request));

                foreach (var header in request.Headers.AllKeys) {
                    var values = request.Headers.GetValues(header);
                    message.Headers.Add(header, values);
                }

                message.Headers.Add("herp", "trolol");
                message.Headers.Host = new Uri(_destination).Host;

                if (requestMethod != HttpMethod.Get) {
                    var originalRequestContent = new StreamReader(request.InputStream).ReadToEnd();
                    message.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(originalRequestContent));
                }

                // TODO - do this properly async
                response = await client.SendAsync(message);
            }

            var destinationResponseString = await response.Content.ReadAsStringAsync();
            //Console.WriteLine("got reply: {0}", destinationResponseString);

            return response;
        }

        private async Task Respond(IncomingHttpRequest incomingRequest, HttpResponseMessage serverResponse) {
            await incomingRequest.RespondWith(serverResponse);
        }

        private HttpMethod ParseHttpMethod(string httpMethod) {
            switch (httpMethod.ToUpper()) {
                case "GET":
                    return HttpMethod.Get;
                case "POST":
                    return HttpMethod.Post;
                case "PUT":
                    return HttpMethod.Put;
                case "DELETE":
                    return HttpMethod.Delete;
                case "OPTIONS":
                    return HttpMethod.Options;
                case "TRACE":
                    return HttpMethod.Trace;
                default:
                    var msg = string.Format("{0} is not a valid HTTP method", httpMethod);
                    throw new ArgumentException("httpMethod", msg);
            }
        }

        public void Stop() {
            _listener.Stop();
        }

        private IObservable<HttpListenerContext> Requests() {
            return Observable.Create(
                (IObserver<HttpListenerContext> observer) => {
                    var loop = true;

                    Task.Run(
                        () => {
                            while (loop) {
                                var context = _listener.GetContext();
                                observer.OnNext(context);
                            }
                        });

                    return () => {
                        // TODO - replace this with proper logging
                        Console.WriteLine("HttpProxy._listener stopped due to Rx stream being closed.");
                        loop = false;
                    };
                });
        }

        public event Action<IncomingHttpRequest> Request;

        protected virtual void OnRequest(IncomingHttpRequest obj) {
            if (Request != null) {
                Request.Invoke(obj);
            }
        }
    }
}