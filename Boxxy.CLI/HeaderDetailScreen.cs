using System;
using Boxxy.Core;

namespace Boxxy
{
    internal class HeaderDetailScreen : IScreen
    {
        private readonly IncomingHttpRequest _request;
        private readonly int _index;

        public HeaderDetailScreen(IncomingHttpRequest request, int index) {
            _request = request;
            _index = index;
        }

        public void Run() {
            while (true) {
                var header = _request.Headers[_index];

                Console.Clear();
                Console.WriteLine("Editing header\n{0}: {1}\n", header.Name, header.Value);
                Console.WriteLine();
                Console.WriteLine("To change the name, type 'n ' followed by the new name.");
                Console.WriteLine("To change the value, type 'v ' followed by the new value.");
                Console.WriteLine("For example 'n Content-Type' will change the header name to 'Content-Type'.");
                Console.WriteLine();
                Console.WriteLine("d) Delete the header");
                Console.WriteLine();

                string input = Console.ReadLine();
                if (input != null) {
                    input = input.Trim();

                    if (input.Length >= 2 && input.Substring(0, 2) == "n ") {
                        header.Name = input.Substring(2);
                    } else if (input.Length >= 2 && input.Substring(0, 2) == "v ") {
                        header.Value = input.Substring(2);
                    } else if (input == "d") {
                        _request.Headers.RemoveAt(_index);
                        break;
                    }
                }
            }
        }
    }
}