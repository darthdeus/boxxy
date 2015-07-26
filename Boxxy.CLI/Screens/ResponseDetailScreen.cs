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
            // TODO - display the response details

            Console.WriteLine("Press ENTER to continue.");
            Console.ReadLine();
        }
    }
}