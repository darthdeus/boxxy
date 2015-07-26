using System;
using Boxxy.Core;

namespace Boxxy.Screens
{
    internal class ChangeMethodScreen : IScreen
    {
        private readonly IncomingHttpRequest _request;

        public ChangeMethodScreen(IncomingHttpRequest request) {
            _request = request;
        }

        public void Run() {
            Console.Clear();
            Console.WriteLine("Changing method for request:\n{0} {1}\n", _request.HttpMethod, _request.Uri);

            Console.WriteLine("Type a new method and press enter (or simply press enter to cancel.)");
            string line = Console.ReadLine();

            if (!string.IsNullOrEmpty(line)) {
                _request.HttpMethod = line.Trim();
            }
        }
    }
}