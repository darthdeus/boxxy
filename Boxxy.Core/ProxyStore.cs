using System.Collections.Generic;
using Boxxy.Core;

namespace Boxxy
{
    public class ProxyStore
    {
        public List<IncomingHttpRequest> Requests { get; private set; }

        public ProxyStore() {
            Requests = new List<IncomingHttpRequest>();
        }
    }
}