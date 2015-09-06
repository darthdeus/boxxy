using System;
using Boxxy.Core;

namespace Boxxy.Screens
{
    internal class ResponseDetailScreen : IScreen
    {
        private readonly IncomingHttpRequest _request;

        public ResponseDetailScreen(IncomingHttpRequest request) {
            _request = request;
        }

        public void Run() {
            Console.WriteLine("Response BODY");
            Console.WriteLine();
            Console.WriteLine(_request.ResponseBody);
            Console.WriteLine();

            Console.WriteLine("Press ENTER to continue.");
            Console.ReadLine();
        }
    }
}