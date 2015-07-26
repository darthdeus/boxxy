using System;
using Boxxy.Core;

namespace Boxxy
{
    internal class ChangeUrlScreen : IScreen
    {
        private readonly IncomingHttpRequest _request;

        public ChangeUrlScreen(IncomingHttpRequest request) {
            _request = request;
        }

        public void Run() {
            Console.Clear();
            Console.WriteLine("Changing URL for request:\n{0} {1}\n", _request.HttpMethod, _request.Uri);

            Console.WriteLine("Type a new URL and press enter (or simply press enter to cancel.)");
            string line = Console.ReadLine();

            if (!string.IsNullOrEmpty(line)) {
                try {
                    var url = new Uri(line);
                    _request.Uri = url;
                } catch (UriFormatException) {
                    Console.WriteLine("Invalid URL format, press ENTER to continue.");
                    Console.ReadLine();

                    Run();
                }
            }
        }
    }
}