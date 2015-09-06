using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Boxxy.Core
{
    public sealed class HttpProxy
    {
        public bool IsRunning { get; private set; }

        private string _destination;
        private HttpListener _listener;
        private CancellationTokenSource _currentTcs;
        private readonly ProxyStore _proxyStore;
        private readonly bool _interactive;

        public HttpProxy(ProxyStore proxyStore, bool interactive) {
            _proxyStore = proxyStore;
            _interactive = interactive;
        }

        public void Start(string prefixUrl, string destination)
        {
            _destination = destination;
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefixUrl);
            IsRunning = false;
 
            _listener.Start();
            IsRunning = true;

            _currentTcs = new CancellationTokenSource();

            // Not really sure why this type isn't inferred properly.
            Task.Run((Action) ListenerLoop, _currentTcs.Token);
        }

        private void ListenerLoop() {
            try {
                while (true) {
                    var context = _listener.GetContext();
                    HandleRequest(context).Wait();
                }
            } catch (HttpListenerException e) {
                // Intentionally left blank, as this only happens when the server is stopped while blocking
                // on GetContext, which happens every time the user force-stops the proxy. This is intentional
                // and based on the MSDN docs.
                Console.WriteLine("Server killed.");
            }
        }

        private async Task HandleRequest(HttpListenerContext context) {
            var incomingRequest = new IncomingHttpRequest(context, _destination);
            _proxyStore.Requests.Add(incomingRequest);
            OnRequest(incomingRequest);

            if (!_interactive) {
                await incomingRequest.Play();
            }
        }
        
        public void Stop() {
            _listener.Stop();
        }

        public event Action<IncomingHttpRequest> Request;

        private void OnRequest(IncomingHttpRequest obj) {
            if (Request != null) {
                Request.Invoke(obj);
            }
        }
    }
}